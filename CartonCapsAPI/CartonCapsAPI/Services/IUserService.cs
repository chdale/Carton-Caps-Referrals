using CartonCapsAPI.Models;

namespace CartonCapsAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<string>> GetReferralCodesAsync();
        Task<User?> GetUserAsync(string userId);
        Task UpdateUserAsync(User user);
        Task<ReferralInformationDto> GetReferralInformationByReferralCodeAsync(string referralCode);
    }
}
