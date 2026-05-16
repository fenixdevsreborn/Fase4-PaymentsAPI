using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaymentsAPI.Infrastructure.Messaging;
using PaymentsAPI.Infrastructure.Messaging.Consumers;
using Shared.Contracts.Events;

public class PurchaseRequestedEventConsumerTests
{
    private readonly Mock<IPaymentNotificationPublisher> _publisherMock = new();

    [Theory]
    [InlineData(100, "Aceito")]
    [InlineData(150, "Aceito")]
    [InlineData(99.99, "Rejeitado")]
    public async Task Consume_ShouldPublishNotification_WithPaymentStatus(decimal amount, string expectedStatus)
    {
        var consumer = new PurchaseRequestedEventConsumer(
            _publisherMock.Object,
            NullLogger<PurchaseRequestedEventConsumer>.Instance);
        var contextMock = new Mock<ConsumeContext<PurchaseRequestedEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new PurchaseRequestedEvent
            {
                UserId = "user-1",
                Email = "user@example.com",
                GameId = "game-1",
                GameName = "Game",
                GameValue = amount,
                Amount = amount,
                RequestedAt = DateTime.UtcNow
            });

        await consumer.Consume(contextMock.Object);

        _publisherMock.Verify(
            x => x.PublishAsync(
                It.Is<EmailNotificationEvent>(e =>
                    e.Title == "Compra processada" &&
                    e.Subtitle == $"Pagamento {expectedStatus}" &&
                    e.Body == $"Seu pagamento foi {expectedStatus}, em caso de duvidas entre em contato com nosso suporte" &&
                    e.Recipient == "payments@fcg,com.br"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
