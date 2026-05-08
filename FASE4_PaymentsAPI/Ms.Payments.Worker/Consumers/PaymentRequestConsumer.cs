using MassTransit;
using Ms.Payments.Application.UseCases;
using Ms.Payments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Worker.Consumers
{
    public class PaymentRequestConsumer : IConsumer<PaymentRequest>
    {
        private readonly ProcessPaymentUseCase _useCase;
        private readonly ILogger<PaymentRequestConsumer> _logger;

        public PaymentRequestConsumer(ProcessPaymentUseCase useCase, ILogger<PaymentRequestConsumer> logger)
        {
            _useCase = useCase;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentRequest> context)
        {
            _logger.LogInformation($"[Consumer] Mensagem recebida para OrderId: {context.Message.OrderId}");

            await _useCase.ExecuteAsync(context.Message);

            _logger.LogInformation($"[Consumer] Processamento concluído para OrderId: {context.Message.OrderId}");
        }
    }
}
