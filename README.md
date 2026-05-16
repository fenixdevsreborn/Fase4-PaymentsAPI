# PaymentsAPI

Worker .NET responsible for processing purchase requests from RabbitMQ.

## Flow

The worker consumes `PurchaseRequestedEvent` messages from `payment-queue`.

Payment rule:

- `Amount >= 100`: approved
- `Amount < 100`: rejected

After processing, it publishes an `EmailNotificationEvent` to `notification-queue`.

## Input Contract

```csharp
public record PurchaseRequestedEvent
{
    public string EventType => "PURCHASE_REQUESTED";
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;
    public decimal GameValue { get; init; }
    public string GameName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime RequestedAt { get; init; }
}
```

## Output Contract

```csharp
public record EmailNotificationEvent
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public string? Sender { get; init; }
}
```

Output message:

```text
Title: Compra processada
Subtitle: Pagamento Aceito|Rejeitado
Body: Seu pagamento foi Aceito|Rejeitado, em caso de duvidas entre em contato com nosso suporte
Recipient: payments@fcg,com.br
```

## Configuration

| Variable | Description | Default |
| --- | --- | --- |
| `RabbitMq__Host` | RabbitMQ host | `localhost` |
| `RabbitMq__Port` | RabbitMQ AMQP port | `5672` |
| `RabbitMq__VirtualHost` | RabbitMQ virtual host | `fiap` |
| `RabbitMq__ExchangeName` | RabbitMQ topic exchange | `fiap.events` |
| `RabbitMq__PaymentQueueName` | Queue consumed by PaymentsAPI | `payment-queue` |
| `RabbitMq__NotificationQueueName` | Queue receiving notification messages | `notification-queue` |
| `RabbitMq__Username` | RabbitMQ username | `guest` |
| `RabbitMq__Password` | RabbitMQ password | `guest` |

## Docker Compose

```bash
cd src
docker compose up --build
```

## Tests

```bash
dotnet test PaymentsAPI.sln
```
