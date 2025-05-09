
namespace CartonCapsAPI.Models
{
    public class ReferralInformationDto
    {
        public string ReferralCode { get; set; }
        public IEnumerable<ReferralDto> Referrals { get; set; }
    }
}
