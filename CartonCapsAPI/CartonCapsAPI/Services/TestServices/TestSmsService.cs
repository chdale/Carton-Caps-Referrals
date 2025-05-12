
namespace CartonCapsAPI.Services.TestServices
{
    public class TestSmsService : ISmsService
    {
        public Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            return Task.FromResult(true);
        }
    }
}
