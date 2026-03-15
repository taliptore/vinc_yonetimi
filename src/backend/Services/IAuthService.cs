using VincYonetim.Api.DTOs.Auth;

namespace VincYonetim.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}
