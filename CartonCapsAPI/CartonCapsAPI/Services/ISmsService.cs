using CartonCapsAPI.Models;
using CartonCapsAPI.Models.DTOs;

namespace CartonCapsAPI.Services;

/// <summary>
/// SMS Service
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Uses an SMS service to send a message to a phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="message"></param>
    /// <returns>A boolean representing success or failure of operation</returns>
    Task<bool> SendMessageAsync(string phoneNumber, string message);
}
