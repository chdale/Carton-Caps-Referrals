using CartonCapsAPI;
using CartonCapsAPI.Controllers;
using CartonCapsAPI.Models;
using CartonCapsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace CartonCapsAPI.Test;

public class UserControllerTest
{
    Mock<ILogger<UserController>> _logger;
    Mock<IUserService> _userService;
    Mock<ISmsService> _smsService;

    UserController _controller;

    public UserControllerTest()
    {
        _logger = new Mock<ILogger<UserController>>();

        _userService = new Mock<IUserService>();
        _smsService = new Mock<ISmsService>();

        #region Mock Data

        // Test User Ids for mock UserService.GetAsync(...)
        var userIdActive = 123;
        var userIdInactive = 124;
        var userIdNullPhoneNumber = 125;
        var userIdInvalidPhoneNumber = 126;
        var userIdNotFound = 127;

        var activateAccountActivationSuccessUserId = 223;
        var activateAccountActivationFailureUserId = 224;
        var activateAccountExpiredTokenUserId = 225;
        var activateAccountInvalidTokenUserId = 226;

        var invalidPhoneNumbers = new string[] { "abc", string.Empty };

        // Test Expiration Dates for use in determining 
        var lateExpirationDate = new DateTime(9999, 12, 31, 23, 59, 59);
        var earlyExpirationDate = new DateTime(1, 1, 1, 0, 0, 0);

        // Test Confirmation Tokens for use in StringUtility.GenerateConfirmationMessage(...)
        var validConfirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        // Test Users from mocked UserService.GetAsync(...)
        var existingUserActive = new User
        {
            UserId = 123,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = null,
            ReferredByUser = null
        };

        var existingUserInactive = new User
        {
            UserId = 124,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null
        };

        var existingUserNullPhoneNumber = new User
        {
            UserId = 125,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = null,
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null
        };

        var existingUserInvalidPhoneNumber = new User
        {
            UserId = 126,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "abc",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null
        };

        var existingUserInvalidConfirmation = new User
        {
            UserId = 126,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "abc",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null
        };


        var existingUserActivationSuccess = new User
        {
            UserId = 223,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = null,
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        var existingUserActivationFailure = new User
        {
            UserId = 224,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = null,
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        var existingUserExpiredToken = new User
        {
            UserId = 225,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = null,
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = earlyExpirationDate
        };

        var existingUserInvalidToken = new User
        {
            UserId = 226,
            FirstName = "Bill",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1991, 1, 1),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = null,
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        //Test phone numbers from StringUtility.SanitizePhoneNumber(...)
        var sanitizedMockPhoneNumber = "9871234567";

        //Test updated users from UserService.UpdateUserConfirmationDataAsync(...)
        var updatedUserActive = new User
        {
            UserId = 123,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = "123ABC",
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        var updatedUserInactive = new User
        {
            UserId = 124,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "(987)123-4567",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.Active,
            ReferralCode = "123ABC",
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        var updatedUserNullPhoneNumber = new User
        {
            UserId = 125,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = null,
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        var updatedUserInvalidPhoneNumber = new User
        {
            UserId = 126,
            FirstName = "John",
            LastName = "Testman",
            PhoneNumber = "abc",
            DateOfBirth = new DateTime(1990, 12, 31),
            ZipCode = 11122,
            AccountStatus = AccountStatus.AwaitingActivation,
            ReferralCode = "123ABC",
            ReferredByUser = null,
            ActivationToken = validConfirmationToken,
            TokenExpirationDate = lateExpirationDate
        };

        //Test confirmation messages from StringUtility.GenerateConfirmationMessage(...)
        var activationMessage = @"
                To unlock all of Carton Caps wonderful features and start investing in the education of our youth, 
                please follow the confirmation link provided below:

                https://www.cartoncapsnetapichallenge2025.com/api/activateaccount?userId={userId}&activationToken={activationToken}";


        #endregion

        #region Mock Methods
        // UserService.GetUserAsync(...) setup
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdActive)).Result)
            .Returns(existingUserActive);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdInactive)).Result)
            .Returns(existingUserInactive);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdNullPhoneNumber)).Result)
            .Returns(existingUserNullPhoneNumber);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdInvalidPhoneNumber)).Result)
            .Returns(existingUserInvalidPhoneNumber);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == userIdNotFound)).Result)
            .Returns((User?)null);

        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == activateAccountActivationSuccessUserId)).Result)
            .Returns(existingUserActivationSuccess);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == activateAccountActivationFailureUserId)).Result)
            .Returns(existingUserActivationFailure);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == activateAccountExpiredTokenUserId)).Result)
            .Returns(existingUserExpiredToken);
        _userService.Setup(us => us.GetUserAsync(It.Is<int>(x => x == activateAccountInvalidTokenUserId)).Result)
            .Returns(existingUserInvalidToken);

        // UserService.UpdateUserConfirmationDataAsync(...) setup
        _userService.Setup(us => us.AssignActivationTokenAsync(It.Is<User>(x => x.UserId == userIdActive)).Result)
            .Returns(updatedUserActive);
        _userService.Setup(us => us.AssignActivationTokenAsync(It.Is<User>(x => x.UserId == userIdInactive)).Result)
            .Returns(updatedUserInactive);
        _userService.Setup(us => us.AssignActivationTokenAsync(It.Is<User>(x => x.UserId == userIdNullPhoneNumber)).Result)
            .Returns(updatedUserNullPhoneNumber);
        _userService.Setup(us => us.AssignActivationTokenAsync(It.Is<User>(x => x.UserId == userIdInvalidPhoneNumber)).Result)
            .Returns(updatedUserInvalidPhoneNumber);

        // SmsService.SendMessageAsync(...) setup
        _smsService.Setup(s => s.SendMessageAsync(It.IsNotIn<string>(invalidPhoneNumbers), It.IsAny<string>()).Result)
            .Returns(true);
        _smsService.Setup(s => s.SendMessageAsync(It.IsIn<string>(invalidPhoneNumbers), It.IsAny<string>()).Result)
            .Returns(false);

        // UserService.UpdateUserAsync(...) setup
        _userService.Setup(us => us.UpdateUserAsync(It.Is<User>(x => x.UserId != activateAccountActivationFailureUserId)).Result)
            .Returns(true);
        _userService.Setup(us => us.UpdateUserAsync(It.Is<User>(x => x.UserId == activateAccountActivationFailureUserId)).Result)
            .Returns(false);
        #endregion

        _controller = new UserController(_logger.Object, _userService.Object, _smsService.Object);
    }

    #region SendSmsConfirmation Unit Tests

    [Fact]
    public void SendSmsConfirmation_User_UserActive()
    {
        //Arrange
        var userId = 123;

        //Act
        var result = _controller.SendSmsConfirmation(userId).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.AccountAlreadyActive, resultValue);
    }

    [Fact]
    public void SendSmsConfirmation_User_UserInactive()
    {
        //Arrange
        var userId = 124;

        //Act
        var result = _controller.SendSmsConfirmation(userId).Result;
        var resultType = result as OkResult;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(resultType);
        Assert.NotNull(resultType);
        Assert.Equal<int>((int)HttpStatusCode.OK, resultType.StatusCode);
    }

    [Fact]
    public void SendSmsConfirmation_User_UserNullPhoneNumber()
    {
        //Arrange
        var userId = 125;

        //Act
        var result = _controller.SendSmsConfirmation(userId).Result;
        var resultType = result as BadRequestObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.ActivationFailedToSendError(userId), resultValue);
    }

    [Fact]
    public void SendSmsConfirmation_User_UserInvalidPhoneNumber()
    {
        //Arrange
        var userId = 126;

        //Act
        var result = _controller.SendSmsConfirmation(userId).Result;
        var resultType = result as BadRequestObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.ActivationFailedToSendError(userId), resultValue);
    }

    [Fact]
    public void SendSmsConfirmation_User_UserDoesNotExist()
    {
        //Arrange
        var userId = 127;

        //Act
        var result = _controller.SendSmsConfirmation(userId).Result;
        var resultType = result as NotFoundObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.UserNotFoundError(userId), resultValue);
    }

    #endregion

    #region ActivateAccount Unit Tests

    [Fact]
    public void ActivateAccount_User_UserActive()
    {
        //Arrange
        var userId = 123;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as OkObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.AccountAlreadyActive, resultValue);
    }

    [Fact]
    public void ActivateAccount_User_ActivationSuccessful()
    {
        //Arrange
        var userId = 223;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as OkResult;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(resultType);
        Assert.NotNull(resultType);
        Assert.Equal<int>((int)HttpStatusCode.OK, resultType.StatusCode);
    }

    [Fact]
    public void ActivateAccount_User_ActivationFailure()
    {
        //Arrange
        var userId = 224;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as ObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, resultType.StatusCode);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.AccountStatusUpdateError(userId), resultValue);
    }

    [Fact]
    public void ActivateAccount_User_TokenExpired()
    {
        //Arrange
        var userId = 225;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as BadRequestObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.TokenExpirationError(userId), resultValue);
    }

    [Fact]
    public void ActivateAccount_User_InvalidToken()
    {
        //Arrange
        var userId = 226;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjIe";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as BadRequestObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.InvalidActivationTokenError(userId), resultValue);
    }

    [Fact]
    public void ActivateAccount_User_UserDoesNotExist()
    {
        //Arrange
        var userId = 127;
        var confirmationToken = "7zZjXtUA2Lkkp08mUzjI";

        //Act
        var result = _controller.ActivateAccount(userId, confirmationToken).Result;
        var resultType = result as NotFoundObjectResult;
        var resultValue = resultType.Value as string;

        //Assert
        Assert.NotNull(result);
        Assert.IsType<string>(resultValue);
        Assert.Equal(Constants.UserMessage.UserNotFoundError(userId), resultValue);
    }

    #endregion
}