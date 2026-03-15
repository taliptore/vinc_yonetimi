namespace VincYonetim.Api.Services;

public interface IEmailService
{
    Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default);
}
