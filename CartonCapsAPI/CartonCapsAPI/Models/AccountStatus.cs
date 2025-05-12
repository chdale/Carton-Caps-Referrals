
using Microsoft.OpenApi.Attributes;

namespace CartonCapsAPI.Models;

public enum AccountStatus
{
    Active = 0,
    [Display("Awaiting Activation")]
    AwaitingActivation = 1
}
