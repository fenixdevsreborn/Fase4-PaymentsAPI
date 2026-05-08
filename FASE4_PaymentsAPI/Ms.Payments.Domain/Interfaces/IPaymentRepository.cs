using Ms.Payments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ms.Payments.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task SaveAsync(PaymentTransaction transaction);
    }
}
