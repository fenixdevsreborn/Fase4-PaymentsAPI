using Microsoft.Extensions.Logging;
using Ms.Payments.Domain.Entities;
using Ms.Payments.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Payments.Infrastructure.Persistence
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(ILogger<PaymentRepository> logger)
        {
            _logger = logger;
        }

        public Task SaveAsync(PaymentTransaction transaction)
        {
            // Aqui integraria o contexto do DynamoDB ou EF Core 
            _logger.LogInformation($"[Database] Salvando transação {transaction.Id} com status {transaction.Status}");
            return Task.CompletedTask;
        }
    }
}
