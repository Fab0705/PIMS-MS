using Microsoft.Extensions.Logging;

namespace PIMS_MS.Modules.Notifications.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger;
    public SmtpEmailSender(ILogger<SmtpEmailSender> logger)
    {
        _logger = logger;
    }
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // Simulamos el envío bloqueando el hilo de forma asíncrona
        _logger.LogInformation("===============================================");
        _logger.LogInformation("📧 ENVIANDO CORREO SMTP A: {To}", to);
        _logger.LogInformation("🔖 ASUNTO: {Subject}", subject);
        _logger.LogInformation("📄 CUERPO:\n{Body}", body);
        _logger.LogInformation("===============================================");

        return Task.CompletedTask;
    }
}