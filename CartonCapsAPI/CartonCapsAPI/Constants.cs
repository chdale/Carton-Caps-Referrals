
namespace CartonCapsAPI;

public static class Constants
{
    public const int ReferralCodeSize = 6;

    public const int ActivationCodeSize = 20;

    public static class UserMessage
    {
        public const string AccountAlreadyActive = "User's account is already active.";

        public const string NullTokenError = "Activation token was null when trying to send activation message.";
        public static string UserNotFoundError(int userId) => $"User: {userId} not found.";
        public static string ActivationFailedToSendError(int userId) => 
            $"Activation failed to send for user: {userId}. Please verify that your phone number looks correct before trying again.";
        public static string ActivationTokenUpdateError(int userId) => $"Failed to update activation token for user: {userId}.";
        public static string AccountStatusUpdateError(int userId) => $"Failed to update account status for user: {userId}.";
        public static string ReferralCodeUpdateError(int userId) => $"Failed to update referral code for user: {userId}.";
        public static string InvalidActivationTokenError(int userId) => $"The activation token provided does not match user: {userId}.";
        public static string TokenExpirationError(int userId) => $"The activation token for user: {userId} has expired.";
        public static string InactiveAccountError(int userId) => $"User: {userId} does not have an activated account.";
    }
}
