using MediatR;
using Microsoft.Extensions.Configuration;
using PIMS_MS.Common.Contracts;
using PIMS_MS.Modules.Notifications.Services;

namespace PIMS_MS.Modules.Notifications.Handlers;
public class NotifyWhenTransferDispatchedHandler : INotificationHandler<TransferDispatchedIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public NotifyWhenTransferDispatchedHandler(IEmailSender emailSender, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task Handle(TransferDispatchedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var subject = $"📦 ALERTA LOGÍSTICA: Traslado {notification.TrackingCode} en tránsito";
        var body = $"El envío con código {notification.TrackingCode} acaba de salir hacia su provincia.\n" +
                   $"Contiene {notification.Items.Count} tipo(s) de repuestos.\n\n" +
                   $"Por favor, esté atento para la recepción.";

        var destinationEmail = _configuration["Notifications:TestEmail"]
            ?? throw new InvalidOperationException("La variable de entorno 'Notifications:TestEmail' no está configurada.");

        // Aquí en un sistema real buscarías el email del supervisor de la provincia de destino.
        // Simulamos un correo fijo por ahora:
        await _emailSender.SendEmailAsync(destinationEmail, subject, body, cancellationToken);
    }
}
