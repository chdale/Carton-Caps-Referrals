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
        /// <returns>A dto containing user's referral code and all active referrals</returns>
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
                var newReferralCode = await _userService.UpdateReferralCodeAsync(user);

                return Ok(new ReferralInformationDto
                {
                    ReferralCode = newReferralCode,
                    Referrals = new List<ReferralDto>()
                });
            }

            var existingReferralInformation = await _userService.GetReferralInformationByReferralCodeAsync(user.ReferralCode);
            return Ok(existingReferralInformation);
        }
    }
}
