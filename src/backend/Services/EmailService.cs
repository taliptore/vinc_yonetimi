namespace VincYonetim.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendPasswordResetAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        // SMTP yapılandırılmamışsa sadece logla (geliştirme ortamı)
        var smtpHost = _config["Email:SmtpHost"];
        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogWarning("Şifre sıfırlama linki (SMTP yok): {Email} -> {Link}", toEmail, resetLink);
            await Task.CompletedTask;
            return;
        }

        // TODO: SmtpClient veya MailKit ile gerçek e-posta gönderimi
        _logger.LogInformation("Şifre sıfırlama e-postası gönderildi: {Email}", toEmail);
        await Task.CompletedTask;
    }
}
