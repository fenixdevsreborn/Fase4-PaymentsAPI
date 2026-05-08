using MassTransit;
using Ms.Payments.Application.UseCases;
using Ms.Payments.Domain.Interfaces;
using Ms.Payments.Worker;
using Ms.Payments.Worker.Consumers;
using MS.Payments.Infrastructure.Messaging;
using MS.Payments.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IMessagePublisher, MassTransitPublisher>();
builder.Services.AddScoped<ProcessPaymentUseCase>();

// 2. Configuração do MassTransit e RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Registra o consumer
    x.AddConsumer<PaymentRequestConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RABBITMQ_HOST"] ?? "localhost";
        var username = builder.Configuration["RABBITMQ_USERNAME"] ?? "guest";
        var password = builder.Configuration["RABBITMQ_PASSWORD"] ?? "guest";

        cfg.Host(host, "/", h => {
            h.Username(username);
            h.Password(password);
        });

        // Configura a fila que este Worker vai escutar
        cfg.ReceiveEndpoint("catalog_payment_requests", e =>
        {
            e.ConfigureConsumer<PaymentRequestConsumer>(context);
            // Configurações extras de resiliência (Retry)
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

var host = builder.Build();
host.Run();