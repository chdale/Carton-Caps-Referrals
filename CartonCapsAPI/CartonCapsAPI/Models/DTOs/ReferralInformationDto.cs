
namespace CartonCapsAPI.Models.DTOs;

public record ReferralInformationDto(string ReferralCode, IEnumerable<ReferralDto> Referrals);
