using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public PaymentTransaction(Guid orderId, decimal amount)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            Amount = amount;
            Status = "Pending";
            CreatedAt = DateTime.UtcNow;
        }

        public void ProcessPayment()
        {
            Status = Amount >= 100 ? "Approved" : "Rejected";
        }
    }
}
