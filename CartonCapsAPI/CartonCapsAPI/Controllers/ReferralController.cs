using Microsoft.AspNetCore.Mvc;

namespace CartonCapsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralController : ControllerBase
    {
        private readonly ILogger<ReferralController> _logger;

        public ReferralController(ILogger<ReferralController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetReferralInformation/{userId}")]
        public async Task<IActionResult> GetReferralInformationByUser(string userId)
        {
            return Ok();
        }
    }
}
