using CartonCapsAPI.Models;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartonCapsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly ISmsService _smsService;

    public UserController(ILogger<UserController> logger, IUserService userService, ISmsService smsService)
    {
        _logger = logger;
        _userService = userService;
        _smsService = smsService;
    }

    /// <summary>
    /// An API endpoint that generates a confirmation token and sends a text message to the phone number for activation
    /// 
    /// The thought behind this method is: In order to dissuade abuse, we would have required unique phone numbers saved to users in order to activate an account.
    /// The referral redemption would only apply to users referred with an activated account. It is not impossible to get fake phone numbers, but it would at least mitigate it heavily.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Either an OK Result or Error Status Code containing messaging with more details</returns>
    [HttpPost("SendSmsConfirmation")]
    public async Task<IActionResult> SendSmsConfirmation([FromBody] int userId)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user == null)
        {
            return NotFound(Constants.UserMessage.UserNotFoundError(userId));
        }
        if (user.AccountStatus == AccountStatus.Active)
        {
            return Ok(Constants.UserMessage.AccountAlreadyActive);
        }

        var updatedUser = await _userService.AssignActivationTokenAsync(user);
        if (updatedUser.ActivationToken is null || updatedUser.TokenExpirationDate is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, Constants.UserMessage.ActivationTokenUpdateError(userId));
        }

        try
        {
            var confirmationSmsMessage = StringUtility.GenerateActivationMessage(updatedUser.UserId, updatedUser.ActivationToken);

            var sanitizedPhoneNumber = StringUtility.SanitizePhoneNumber(updatedUser.PhoneNumber ?? string.Empty);

            var isSuccess = await _smsService.SendMessageAsync(sanitizedPhoneNumber, confirmationSmsMessage);

            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(Constants.UserMessage.ActivationFailedToSendError(userId));
            }
        }
        catch (NullReferenceException ex)
        {
            return BadRequest(Constants.UserMessage.NullTokenError);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// An API endpoint that accepts a confirmation token for a user id, verifies it is valid and not expired, then activates the account
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activationToken"></param>
    /// <returns></returns>
    [HttpPost("ActivateAccount")]
    public async Task<IActionResult> ActivateAccount([FromQuery] int userId, [FromQuery] string activationToken)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user == null)
        {
            return NotFound(Constants.UserMessage.UserNotFoundError(userId));
        }
        if (user.AccountStatus == AccountStatus.Active)
        {
            return Ok(Constants.UserMessage.AccountAlreadyActive);
        }

        if (user.ActivationToken is not null && user.ActivationToken.Equals(activationToken))
        {
            if (user.TokenExpirationDate.HasValue && DateTime.UtcNow < user.TokenExpirationDate.Value)
            {
                user.AccountStatus = AccountStatus.Active;
                var isSuccess = await _userService.UpdateUserAsync(user);
                if (isSuccess)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, Constants.UserMessage.AccountStatusUpdateError(userId));
                }
            }
            else
            {
                return BadRequest(Constants.UserMessage.TokenExpirationError(userId));
            }
        }
        else
        {
            return BadRequest(Constants.UserMessage.InvalidActivationTokenError(userId));
        }
    }
}
