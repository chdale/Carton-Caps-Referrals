using CartonCapsAPI;
using CartonCapsAPI.Controllers;
using CartonCapsAPI.Models;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Moq;

namespace CartonCapsAPITest
{
    public class ReferralControllerTest
    {
        Mock<ILogger<ReferralController>> _logger;
        Mock<IUserService> _userService;
        ReferralController _controller;

        public ReferralControllerTest()
        {
            _logger = new Mock<ILogger<ReferralController>>();

            _userService = new Mock<IUserService>();

            #region Mock Data

            var uniqueReferralCode = "ABC123";

            var existingReferralCodes = new List<string> { "123ABC", "XYZ123", "123XYZ" };

            var userIdWithReferralCode = new string[] { "B123" };

            var userIdWithNoReferralCode = new string[] { "A123" };

            var userIdNotFound = "C123";

            var existingUserHasReferralCode = new User
            {
                UserId = "B123",
                FirstName = "John",
                LastName = "Testman",
                PhoneNumber = "(987)123-4567",
                DateOfBirth = new DateTime(1990, 12, 31),
                ZipCode = 11122,
                AccountStatus = AccountStatus.Active,
                ReferralCode = "123ABC",
                ReferredByCode = null
            };

            var existingUserHasNoReferralCode = new User
            {
                UserId = "A123",
                FirstName = "Bill",
                LastName = "Testman",
                PhoneNumber = "(987)123-1234",
                DateOfBirth = new DateTime(1991, 1, 1),
                ZipCode = 11122,
                AccountStatus = AccountStatus.Active,
                ReferralCode = null,
                ReferredByCode = null
            };

            var referralInformationWithReferees = new ReferralInformationDto
            {
                ReferralCode = "123ABC",
                Referrals = new List<ReferralDto>
                {
                    new ReferralDto 
                    {
                        DisplayName = "Emily F.",
                        AccountStatusDisplay = AccountStatus.Active.GetDisplayName()
                    },
                    new ReferralDto
                    {
                        DisplayName = "Rodney S.",
                        AccountStatusDisplay = AccountStatus.Blocked.GetDisplayName()
                    },
                    new ReferralDto
                    {
                        DisplayName = "Harry I.",
                        AccountStatusDisplay = AccountStatus.AwaitingConfirmation.GetDisplayName()
                    }
                }
            };

            #endregion

            #region Mock Methods

            _userService.Setup(us => us.GetUserAsync(It.IsIn<string>(userIdWithReferralCode)).Result)
                .Returns(existingUserHasReferralCode);
            _userService.Setup(us => us.GetUserAsync(It.IsIn<string>(userIdWithNoReferralCode)).Result)
                .Returns(existingUserHasNoReferralCode);
            _userService.Setup(us => us.GetUserAsync(It.Is<string>(x => x.Equals(userIdNotFound))).Result)
                .Returns((User?)null);
            _userService.Setup(us => us.UpdateReferralCodeAsync(It.IsAny<User>()).Result)
                .Returns(uniqueReferralCode);
            _userService.Setup(us => us.GetReferralInformationByReferralCodeAsync(It.IsIn<string>(existingReferralCodes)).Result)
                .Returns(referralInformationWithReferees);
            #endregion

            _controller = new ReferralController(_logger.Object, _userService.Object);
        }

        [Fact]
        public void GetReferralInformationByUser_Referral_NewReferralCode()
        {
            //Arrange
            var userIdWithNoReferralCode = "A123";

            //Act
            var result = _controller.GetReferralInformationByUser(userIdWithNoReferralCode).Result;
            var resultType = result as OkObjectResult;
            var resultValue = resultType.Value as ReferralInformationDto;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ReferralInformationDto>(resultValue);
            Assert.NotNull(resultValue.Referrals);
            Assert.Empty(resultValue.Referrals);
        }

        [Fact]
        public void GetReferralInformationByUser_Referral_ExistingReferralCode()
        {
            //Arrange
            var userIdWithReferralCodeAndReferees = "B123";

            //Act
            var result = _controller.GetReferralInformationByUser(userIdWithReferralCodeAndReferees).Result;
            var resultType = result as OkObjectResult;
            var resultValue = resultType.Value as ReferralInformationDto;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ReferralInformationDto>(resultValue);
            Assert.NotNull(resultValue.Referrals);
            Assert.NotEmpty(resultValue.Referrals);
            Assert.Equal(3, resultValue.Referrals.Count());
        }

        [Fact]
        public void GetReferralInformationByUser_Referral_UserNotFound()
        {
            //Arrange
            var userIdWithReferralCodeAndReferees = "C123";

            //Act
            var result = _controller.GetReferralInformationByUser(userIdWithReferralCodeAndReferees).Result;
            var resultType = result as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}