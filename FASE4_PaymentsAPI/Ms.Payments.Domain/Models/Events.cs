using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Domain.Models
{
    public record PaymentRequest(Guid OrderId, decimal Amount);
    public record PaymentProcessed(Guid OrderId, string Status);
}