
namespace CartonCapsAPI.Models.DTOs;

/// <summary>
/// DTO with the display name and account status of referred user
/// </summary>
/// <param name="DisplayName"></param>
/// <param name="AccountStatusDisplay"></param>
public record ReferralDto(string DisplayName, string AccountStatusDisplay);
