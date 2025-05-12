using System.Text;

namespace CartonCapsAPI.Utilities;

public static class StringUtility
{
    private static string validCharacters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string GenerateActivationMessage(int userId, string activationToken)
    {
        return $@"
                To unlock all of Carton Caps wonderful features and start investing in the education of our youth, 
                please follow the confirmation link provided below:

                https://www.cartoncapsnetapichallenge2025.com/api/activateaccount?userId={userId}&activationToken={activationToken}";
    }

    public static string GenerateCode(int length, IEnumerable<string> unusableCodes = null)
    {
        Random rand = new Random();
        StringBuilder randomSb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            var randomValidCharacter = validCharacters[rand.Next(validCharacters.Length)];
            randomSb.Append(randomValidCharacter);
        }

        var generatedReferralCode = randomSb.ToString();
        if (unusableCodes != null && unusableCodes.Contains(generatedReferralCode))
        {
            return GenerateCode(length, unusableCodes);
        }
        else
        {
            return generatedReferralCode;
        }
    }

    public static string SanitizePhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(x => char.IsDigit(x)).ToArray());
    }

    public static string GetUserDisplayName(string firstName, string lastName)
    {
        var lastInitial = lastName.First().ToString();
        return $"{firstName} {lastInitial}.";
    }
}
