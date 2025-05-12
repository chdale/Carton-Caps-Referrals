using System.ComponentModel.DataAnnotations;

namespace CartonCapsAPI.Models;

public enum AccountStatus
{
    Active = 0,
    [Display(Name = "Awaiting Activation")]
    AwaitingActivation = 1
}
