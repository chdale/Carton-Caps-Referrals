namespace CartonCapsAPI.Models.DTOs;

/// <summary>
/// DTO dictating how the app will launch, and with the referral code if needed
/// </summary>
/// <param name="AppLaunchStyle"></param>
/// <param name="ReferralCode"></param>
public record AppLaunchDto(AppLaunchStyle AppLaunchStyle, string? ReferralCode = null);
