using CartonCapsAPI.Models;
using CartonCapsAPI.Models.DTOs;
using CartonCapsAPI.Utilities;
using Microsoft.OpenApi.Extensions;

namespace CartonCapsAPI.Services.TestServices
{
    public class TestUserService : IUserService
    {
        private List<User> existingUsers = new List<User>()
        {
            new User
            {
                UserId = 1,
                FirstName = "Bill",
                LastName = "Testman",
                PhoneNumber = "(987)123-4567",
                DateOfBirth = new DateTime(1991, 1, 1),
                ZipCode = 11122,
                AccountStatus = AccountStatus.Active,
                ReferralCode = "abc123",
                ReferredByUser = null,
                ActivationToken = null,
                TokenExpirationDate = null
            },
            new User
            {
                UserId = 2,
                FirstName = "Test",
                LastName = "Billman",
                PhoneNumber = "(404)123-4567",
                DateOfBirth = new DateTime(1991, 1, 1),
                ZipCode = 22211,
                AccountStatus = AccountStatus.Active,
                ReferralCode = null,
                ReferredByUser = null,
                ActivationToken = null,
                TokenExpirationDate = null
            },
            new User
            {
                UserId = 3,
                FirstName = "Inactive",
                LastName = "User",
                PhoneNumber = "(101)123-4567",
                DateOfBirth = new DateTime(1991, 1, 1),
                ZipCode = 33344,
                AccountStatus = AccountStatus.AwaitingActivation,
                ReferralCode = null,
                ReferredByUser = null,
                ActivationToken = null,
                TokenExpirationDate = null
            },
            new User
            {
                UserId = 4,
                FirstName = "Activate",
                LastName = "User",
                PhoneNumber = "(202)123-4567",
                DateOfBirth = new DateTime(1991, 1, 1),
                ZipCode = 44433,
                AccountStatus = AccountStatus.AwaitingActivation,
                ReferralCode = null,
                ReferredByUser = null,
                ActivationToken = "abc",
                TokenExpirationDate = new DateTime(9999, 12, 31, 23, 59, 59)
            },
        };

        private List<User> referredUsers = new List<User>
        {
            new User
            {
                FirstName = "Rhonda",
                LastName = "Patterson",
                ReferredByUser = 1,
                AccountStatus = AccountStatus.Active,
            },
            new User
            {
                FirstName = "Bogdan",
                LastName = "Anderson",
                ReferredByUser = 1,
                AccountStatus = AccountStatus.AwaitingActivation,
            }
        };
        
        public Task<User> AssignActivationTokenAsync(User user)
        {
            var activatedUser = new User()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ZipCode = user.ZipCode,
                AccountStatus = user.AccountStatus,
                ReferralCode = user.ReferralCode,
                ReferredByUser = user.ReferredByUser,
                ActivationToken = StringUtility.GenerateCode(Constants.ActivationCodeSize),
                TokenExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            return Task.FromResult(activatedUser);
        }

        public Task<IEnumerable<string>> GetReferralCodesAsync()
        {
            return Task.FromResult(existingUsers.Where(x => x.ReferralCode != null).Select(x => x.ReferralCode.GetValueOrDefault()).AsEnumerable());
        }

        public Task<ReferralInformationDto> GetReferralInformationByUserIdAsync(int userId, string referralCode)
        {
            return Task.FromResult(new ReferralInformationDto(
                referralCode,
                referredUsers
                    .Where(x => x.ReferredByUser != null && x.ReferredByUser.Equals(userId))
                    .Select(x => new ReferralDto(StringUtility.GetUserDisplayName(x.FirstName, x.LastName), x.AccountStatus.GetDisplayName()))));
        }

        public Task<User?> GetUserAsync(int userId)
        {
            return Task.FromResult(existingUsers.SingleOrDefault(x => x.UserId.Equals(userId)));
        }

        public Task<string> UpdateReferralCodeAsync(User user)
        {
            return Task.FromResult(StringUtility.GenerateCode(Constants.ReferralCodeSize, GetReferralCodesAsync().Result));
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ValidateReferralCodeAsync(string referralCode)
        {
            return Task.FromResult(referralCode.Count() == Constants.ReferralCodeSize);
        }
    }
}
