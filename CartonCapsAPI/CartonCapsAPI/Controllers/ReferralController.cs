using CartonCapsAPI.Models.DTOs;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartonCapsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly ILogger<ReferralController> _logger;
    private readonly IUserService _userService;

    public ReferralController(ILogger<ReferralController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// An API endpoint that gets the referral code and active referrals they have
    /// 
    /// This method assumes that since this is a new feature, that there will be users who need to have referral codes generated
    /// as they did not have referral codes prior to the creation of the feature. It is assumed that the new field was made nullable for 
    /// all existing users.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Either an OK Result with a dto containing user's referral code and all active referrals or Error Status Code containing messaging with more details</returns>
    [HttpGet("GetReferralInformation/{userId}")]
    public async Task<IActionResult> GetReferralInformationByUser(int userId)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user == null)
        {
            return NotFound(Constants.UserMessage.UserNotFoundError(userId));
        }
        if (user.AccountStatus == Models.AccountStatus.AwaitingActivation)
        {
            return Unauthorized(Constants.UserMessage.InactiveAccountError(userId));
        }

        if (string.IsNullOrWhiteSpace(user.ReferralCode))
        {
            var newReferralCode = await _userService.UpdateReferralCodeAsync(user);

            if (string.IsNullOrEmpty(newReferralCode))
            { 
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.UserMessage.ReferralCodeUpdateError(userId));
            } 
            else
            {
                return Ok(new ReferralInformationDto(newReferralCode, new List<ReferralDto>()));
            }
        }

        var existingReferralInformation = await _userService.GetReferralInformationByReferralCodeAsync(user.ReferralCode);
        return Ok(existingReferralInformation);
    }
}
