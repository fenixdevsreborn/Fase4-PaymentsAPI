using Ms.Payments.Domain.Entities;
using Ms.Payments.Domain.Interfaces;
using Ms.Payments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Application.UseCases
{
    public class ProcessPaymentUseCase
    {
        private readonly IPaymentRepository _repository;
        private readonly IMessagePublisher _publisher;

        public ProcessPaymentUseCase(IPaymentRepository repository, IMessagePublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task ExecuteAsync(PaymentRequest request)
        {
            var transaction = new PaymentTransaction(request.OrderId, request.Amount);

            transaction.ProcessPayment();

            await _repository.SaveAsync(transaction);

            var resultEvent = new PaymentProcessed(
                transaction.OrderId,
                transaction.Status
            );

            await _publisher.PublishResultAsync(resultEvent);
        }
    }
}
