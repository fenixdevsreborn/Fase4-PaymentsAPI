using Ms.Payments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishResultAsync(PaymentProcessed resultEvent);
    }
}
