namespace Shared.Contracts.Events;

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
