using System.ComponentModel.DataAnnotations;

namespace CartonCapsAPI.Models
{
    public enum AccountStatus
    {
        Active = 0,
        Blocked = 1,
        [Display(Name = "Awaiting Confirmation")]
        AwaitingConfirmation = 2
    }
}
