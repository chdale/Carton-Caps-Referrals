
namespace CartonCapsAPI.Models.DTOs;

/// <summary>
/// DTO with user's referral code and display information of user's they referred
/// </summary>
/// <param name="ReferralCode"></param>
/// <param name="Referrals"></param>
public record ReferralInformationDto(string ReferralCode, IEnumerable<ReferralDto> Referrals);
