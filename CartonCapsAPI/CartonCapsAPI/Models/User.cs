﻿using System.Diagnostics.CodeAnalysis;

namespace CartonCapsAPI.Models
{
    /// <summary>
    /// User object, several of the nullable values are due to the expectation of new fields added for referral feature
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }   
        public int ZipCode { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public string? ReferralCode { get; set; }
        public int? ReferredByUser { get; set;}
        public string? ActivationToken { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
    }
}
