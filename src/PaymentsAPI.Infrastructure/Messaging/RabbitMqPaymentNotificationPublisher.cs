using RabbitMQ.Client;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace PaymentsAPI.Infrastructure.Messaging;

public sealed class RabbitMqPaymentNotificationPublisher : IPaymentNotificationPublisher, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly string _exchangeName;
    private readonly string _notificationQueueName;
    private IChannel? _channel;

    public RabbitMqPaymentNotificationPublisher()
    {
        _exchangeName = Environment.GetEnvironmentVariable("RabbitMq__ExchangeName") ?? "fiap.events";
        _notificationQueueName =
            Environment.GetEnvironmentVariable("RabbitMq__NotificationQueueName")
            ?? "notification-queue";

        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMq__Port") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RabbitMq__Username") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest",
            VirtualHost = Environment.GetEnvironmentVariable("RabbitMq__VirtualHost") ?? "fiap"
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync(
        EmailNotificationEvent message,
        CancellationToken cancellationToken = default)
    {
        await EnsureChannelAsync(cancellationToken);

        await _channel!.QueueDeclareAsync(
            queue: _notificationQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: _notificationQueueName,
            exchange: _exchangeName,
            routingKey: _notificationQueueName,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: _notificationQueueName,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is null || _channel.IsClosed)
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        await _connection.DisposeAsync();
    }
}
