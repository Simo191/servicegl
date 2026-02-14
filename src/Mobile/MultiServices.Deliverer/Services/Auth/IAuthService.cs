using MultiServices.Deliverer.Models;

namespace MultiServices.Deliverer.Services.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(object registrationData);
    Task<bool> CheckAuthAsync();
    Task<bool> IsOnboardingCompleteAsync();
    Task<bool> IsVerifiedAsync();
    Task LogoutAsync();
    Task<DelivererProfile?> GetDelivererProfileAsync();
    Task<string?> GetTokenAsync();
}
