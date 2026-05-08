using MassTransit;
using Ms.Payments.Domain.Interfaces;
using Ms.Payments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Payments.Infrastructure.Messaging
{
    public class MassTransitPublisher : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishResultAsync(PaymentProcessed resultEvent)
        {
            await _publishEndpoint.Publish(resultEvent);
        }
    }
}
