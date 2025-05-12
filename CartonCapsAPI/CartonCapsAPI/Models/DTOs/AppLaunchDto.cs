namespace CartonCapsAPI.Models.DTOs
{
    public record AppLaunchDto(AppLaunchStyle AppLaunchStyle, string? ReferralCode = null);
}
