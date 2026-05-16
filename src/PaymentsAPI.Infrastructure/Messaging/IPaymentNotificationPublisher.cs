using Shared.Contracts.Events;

namespace PaymentsAPI.Infrastructure.Messaging;

public interface IPaymentNotificationPublisher
{
    Task PublishAsync(EmailNotificationEvent message, CancellationToken cancellationToken = default);
}
