namespace CartonCapsAPI
{
    public interface IStringUtility
    {
        /// <summary>
        /// Uses a list of existing referral codes and desired length to generate a referral code
        /// </summary>
        /// <param name="referralCodeLength"></param>
        /// <param name="existingReferralCodes"></param>
        /// <returns>A string representation of a referral code with the length of {referralCodeLength}</returns>
        string GenerateUniqueReferralCode(int referralCodeLength, IEnumerable<string> existingReferralCodes);
    }
}
