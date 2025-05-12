using CartonCapsAPI;
using CartonCapsAPI.Controllers;
using CartonCapsAPI.Models;
using CartonCapsAPI.Models.DTOs;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Moq;

namespace CartonCapsAPI.Test;

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

        var userIdWithNewReferralCode = 123;
        var userIdWithExistingReferralCode = 124;
        var userIdNotFound = 125;
        var userIdFailReferralCodeUpdate = 126;
        var userIdInactiveAccount = 127;

        var existingUserNewReferralCode = new User
        {
            UserId = 123,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-1234",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = null,
            ReferredByUser = null
        };

        var existingUserExistingReferralCode = new User
        {
            UserId = 124,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = "123ABC",
            ReferredByUser = null
        };

        var existingUserFailToUpdateReferralCode = new User
        {
            UserId = 126,
            FirstName = "Jed",
            LastName = "Testman",
            PhoneNumber = "(987)123-1234",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = null,
            ReferredByUser = null
        };

        var existingUserInactiveAccount = new User
        {
            UserId = 127,
            FirstName = "Mud",
            LastName = "Testman",
            PhoneNumber = "(987)123-1234",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = null,
            ReferredByUser = null
        };

        var referralInformationWithReferees = new ReferralInformationDto(
            "123ABC",
            new List<ReferralDto>
            {
                new ReferralDto("Emily F.", AccountStatus.Active.GetDisplayName()),
                new ReferralDto("Rodney S.", AccountStatus.Active.GetDisplayName()),
                new ReferralDto("Harry I.", AccountStatus.Active.GetDisplayName())
            });

        #endregion

        #region Mock Methods

        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdWithExistingReferralCode)).Result)
            .Returns(existingUserExistingReferralCode);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdWithNewReferralCode)).Result)
            .Returns(existingUserNewReferralCode);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdNotFound)).Result)
            .Returns((User?)null);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdFailReferralCodeUpdate)).Result)
            .Returns(existingUserFailToUpdateReferralCode);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdInactiveAccount)).Result)
            .Returns(existingUserInactiveAccount);

        _userService.Setup(us => us.UpdateReferralCodeAsync(It.Is<User>(x => x.UserId != userIdFailReferralCodeUpdate)).Result)
            .Returns(uniqueReferralCode);
        _userService.Setup(us => us.UpdateReferralCodeAsync(It.Is<User>(x => x.UserId == userIdFailReferralCodeUpdate)).Result)
            .Returns(string.Empty);

        _userService.Setup(us => us.GetReferralInformationByUserIdAsync(It.IsAny<int>(), It.IsIn<string>(existingReferralCodes)).Result)
            .Returns(referralInformationWithReferees);
        #endregion

        _controller = new ReferralController(_logger.Object, _userService.Object);
    }

    [Fact]
    public void GetReferralInformationByUser_Referral_NewReferralCode()
    {
        //Arrange
        var userIdWithNoReferralCode = 123;

        //Act
        var result = _controller.GetReferralInformationByUser(userIdWithNoReferralCode).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as ReferralInformationDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<ReferralInformationDto>(resultValue);
        Assert.NotNull(resultValue.Referrals);
        Assert.Empty(resultValue.Referrals);
    }

    [Fact]
    public void GetReferralInformationByUser_Referral_ExistingReferralCode()
    {
        //Arrange
        var userIdWithReferralCodeAndReferees = 124;

        //Act
        var result = _controller.GetReferralInformationByUser(userIdWithReferralCodeAndReferees).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as ReferralInformationDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<ReferralInformationDto>(resultValue);
        Assert.NotNull(resultValue.Referrals);
        Assert.NotEmpty(resultValue.Referrals);
        Assert.Equal(3, resultValue.Referrals.Count());
    }

    [Fact]
    public void GetReferralInformationByUser_Referral_UserNotFound()
    {
        //Arrange
        var userIdWithReferralCodeAndReferees = 125;

        //Act
        var result = _controller.GetReferralInformationByUser(userIdWithReferralCodeAndReferees).Result;
        var resultType = result as NotFoundObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, resultType.StatusCode);
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(Constants.UserMessage.UserNotFoundError(userIdWithReferralCodeAndReferees), resultValue);
    }

    [Fact]
    public void GetReferralInformationByUser_Referral_FailedToUpdateReferralCodes()
    {
        //Arrange
        var userIdFailsReferralCodeUpdate = 126;

        //Act
        var result = _controller.GetReferralInformationByUser(userIdFailsReferralCodeUpdate).Result;
        var resultType = result as ObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultType.StatusCode);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.ReferralCodeUpdateError(userIdFailsReferralCodeUpdate), resultValue);
    }

    [Fact]
    public void GetReferralInformationByUser_Referral_InactiveAccountForbidden()
    {
        //Arrange
        var userIdInactiveAccount = 127;

        //Act
        var result = _controller.GetReferralInformationByUser(userIdInactiveAccount).Result;
        var resultType = result as ObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, resultType.StatusCode);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.InactiveAccountError(userIdInactiveAccount), resultValue);
    }
}