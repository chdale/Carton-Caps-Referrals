using CartonCapsAPI.Models;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartonCapsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralController : ControllerBase
    {
        private readonly ILogger<ReferralController> _logger;
        private readonly IUserService _userService;
        private readonly IStringUtility _stringUtility;

        public ReferralController(ILogger<ReferralController> logger, IUserService userService, IStringUtility stringUtility)
        {
            _logger = logger;
            _userService = userService;
            _stringUtility = stringUtility;
        }

        [HttpGet("GetReferralInformation/{userId}")]
        public async Task<IActionResult> GetReferralInformationByUser(string userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(user.ReferralCode))
            {
                var existingReferralCodes = await _userService.GetReferralCodesAsync();
                var uniqueReferralCode = _stringUtility.GenerateUniqueReferralCode(Constants.ReferralCodeSize, existingReferralCodes);
                user.ReferralCode = uniqueReferralCode;
                await _userService.UpdateUserAsync(user);

                return Ok(new ReferralInformationDto
                {
                    ReferralCode = uniqueReferralCode,
                    Referrals = new List<ReferralDto>()
                });
            }

            var referralInformation = await _userService.GetReferralInformationByReferralCodeAsync(user.ReferralCode);
            return Ok(referralInformation);
        }
    }
}
