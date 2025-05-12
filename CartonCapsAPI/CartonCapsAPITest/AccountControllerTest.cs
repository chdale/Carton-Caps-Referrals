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

namespace CartonCapsAPITest;

public class AccountControllerTest
{
    Mock<ILogger<AccountController>> _logger;
    Mock<IUserService> _userService;
    AccountController _controller;

    public AccountControllerTest()
    {
        _logger = new Mock<ILogger<AccountController>>();

        _userService = new Mock<IUserService>();

        #region Mock Data

        var validReferralCode = "zZ8j2K";

        #endregion

        #region Mock Methods

        _userService.Setup(us => us.ValidateReferralCodeAsync(It.Is<string>(x => x.Equals(validReferralCode))).Result)
            .Returns(true);
        _userService.Setup(us => us.ValidateReferralCodeAsync(It.Is<string>(x => !x.Equals(validReferralCode))).Result)
            .Returns(false);

        #endregion

        _controller = new AccountController(_logger.Object, _userService.Object);
    }

    [Fact]
    public void GetApplicationLaunchData_Account_UserLoggedIn()
    {
        //Arrange
        var loggedInStatus = true;

        //Act
        var result = _controller.GetApplicationLaunchData(loggedInStatus).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as AppLaunchDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<AppLaunchDto>(resultValue);
        Assert.NotNull(resultValue);
        Assert.Equal(AppLaunchStyle.Normal,resultValue.AppLaunchStyle);
        Assert.Null(resultValue.ReferralCode);
    }

    [Fact]
    public void GetApplicationLaunchData_Account_UserLoggedInWithReferralCode()
    {
        //Arrange
        var loggedInStatus = true;
        var referralCode = "a9sx9K";

        //Act
        var result = _controller.GetApplicationLaunchData(loggedInStatus, referralCode).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as AppLaunchDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<AppLaunchDto>(resultValue);
        Assert.NotNull(resultValue);
        Assert.Equal(AppLaunchStyle.Normal, resultValue.AppLaunchStyle);
        Assert.Null(resultValue.ReferralCode);
    }

    [Fact]
    public void GetApplicationLaunchData_Account_NewUserNoReferral()
    {
        //Arrange
        var loggedInStatus = false;

        //Act
        var result = _controller.GetApplicationLaunchData(loggedInStatus).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as AppLaunchDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<AppLaunchDto>(resultValue);
        Assert.NotNull(resultValue);
        Assert.Equal(AppLaunchStyle.FTE, resultValue.AppLaunchStyle);
        Assert.Null(resultValue.ReferralCode);
    }

    [Fact]
    public void GetApplicationLaunchData_Account_NewUserWithReferral()
    {
        //Arrange
        var loggedInStatus = false;
        var referralCode = "zZ8j2K";

        //Act
        var result = _controller.GetApplicationLaunchData(loggedInStatus, referralCode).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as AppLaunchDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<AppLaunchDto>(resultValue);
        Assert.NotNull(resultValue);
        Assert.Equal(AppLaunchStyle.Referred, resultValue.AppLaunchStyle);
        Assert.NotNull(resultValue.ReferralCode);
        Assert.Equal(referralCode, resultValue.ReferralCode);
    }

    [Fact]
    public void GetApplicationLaunchData_Account_NewUserWithInvalidReferral()
    {
        //Arrange
        var loggedInStatus = false;
        var referralCode = "zZ8j2Kx";

        //Act
        var result = _controller.GetApplicationLaunchData(loggedInStatus, referralCode).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as AppLaunchDto;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode);
        Assert.IsType<AppLaunchDto>(resultValue);
        Assert.NotNull(resultValue);
        Assert.Equal(AppLaunchStyle.FTE, resultValue.AppLaunchStyle);
        Assert.Null(resultValue.ReferralCode);
    }
}