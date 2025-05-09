namespace CartonCapsAPI
{
    public interface IStringUtility
    {
        string GenerateUniqueReferralCode(int referralCodeLength, IEnumerable<string> existingReferralCodes);
    }
}
