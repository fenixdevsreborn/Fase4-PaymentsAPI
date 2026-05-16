using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentsAPI.Infrastructure.Messaging;
using Shared.Contracts.Events;

namespace PaymentsAPI.Infrastructure.Messaging.Consumers;

public class PurchaseRequestedEventConsumer : IConsumer<PurchaseRequestedEvent>
{
    private readonly IPaymentNotificationPublisher _publisher;
    private readonly ILogger<PurchaseRequestedEventConsumer> _logger;

    public PurchaseRequestedEventConsumer(
        IPaymentNotificationPublisher publisher,
        ILogger<PurchaseRequestedEventConsumer> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PurchaseRequestedEvent> context)
    {
        var purchase = context.Message;
        var successfulPayment = purchase.Amount >= 100;
        var status = successfulPayment ? "Aceito" : "Rejeitado";

        _logger.LogInformation(
            "[PaymentsAPI] Purchase requested. GameId: {GameId}, Amount: {Amount}, Status: {Status}",
            purchase.GameId,
            purchase.Amount,
            status);

        await _publisher.PublishAsync(
            new EmailNotificationEvent
            {
                Title = "Compra processada",
                Subtitle = $"Pagamento {status}",
                Body = $"Seu pagamento foi {status}, em caso de duvidas entre em contato com nosso suporte",
                Recipient = "payments@fcg,com.br"
            },
            context.CancellationToken);
    }
}
