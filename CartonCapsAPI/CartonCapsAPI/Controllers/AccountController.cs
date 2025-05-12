using CartonCapsAPI.Models;
using CartonCapsAPI.Models.DTOs;
using CartonCapsAPI.Services;
using CartonCapsAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CartonCapsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;

    public AccountController(ILogger<AccountController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Takes login status of launching users application and determines how the application should launch, between:
    /// Normal login, First Time Experience, or Referred First Time Experience
    /// </summary>
    /// <param name="isLoggedIn"></param>
    /// <param name="referralCode"></param>
    /// <returns>A dto containing data on how the application should launch and referral code if applicable</returns>
    [HttpGet("GetApplicationLaunchData/{isLoggedIn}")]
    public async Task<IActionResult> GetApplicationLaunchData(bool isLoggedIn, [FromQuery] string? referralCode = null)
    {
        if (isLoggedIn)
        {
            return Ok(new AppLaunchDto(AppLaunchStyle.Normal));
        }
        else
        {
            if (referralCode != null && await _userService.ValidateReferralCodeAsync(referralCode.GetValueOrDefault()))
            {
                return Ok(new AppLaunchDto(AppLaunchStyle.Referred, referralCode));
            }
            else
            {
                return Ok(new AppLaunchDto(AppLaunchStyle.FTE));
            }
        }
    }
}
