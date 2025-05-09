using System.Diagnostics.CodeAnalysis;

namespace CartonCapsAPI.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [AllowNull]
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }   
        public int ZipCode { get; set; }
        public AccountStatus AccountStatus { get; set; }
        [AllowNull]
        public string ReferralCode { get; set; }
        [AllowNull]
        public string ReferredByCode { get; set;}
    }
}
