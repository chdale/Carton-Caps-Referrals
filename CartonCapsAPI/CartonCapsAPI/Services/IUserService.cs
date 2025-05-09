using CartonCapsAPI.Models;

namespace CartonCapsAPI.Services
{
    /// <summary>
    /// User Service interface handles CRUD operations involving users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Used by UpdateReferralCodeAsync(...) to ensure newly generated referral code is unique
        /// </summary>
        /// <returns>An IEnumerable of all existing referral codes</returns>
        Task<IEnumerable<string>> GetReferralCodesAsync();

        /// <summary>
        /// Get User with given userId if exists
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A user object or null if it does not exist</returns>
        Task<User?> GetUserAsync(string userId);

        /// <summary>
        /// Gets referral information for use on Carton Caps user referrals page
        /// </summary>
        /// <param name="referralCode"></param>
        /// <returns>A dto containing the user's referral code and display information for all active referrals they have</returns>
        Task<ReferralInformationDto> GetReferralInformationByReferralCodeAsync(string referralCode);

        /// <summary>
        /// Creates new unique referral code for user and returns it after successfully updating the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A string representation of a new referral code</returns>
        Task<string> UpdateReferralCodeAsync(User user);

        /// <summary>
        /// Used by UpdateReferralCodeAsync(...) to update the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A boolean representing the success of the operation</returns>
        Task<bool> UpdateUserAsync(User user);
    }
}
