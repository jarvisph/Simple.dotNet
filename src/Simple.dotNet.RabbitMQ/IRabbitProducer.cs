using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Dependency;

namespace Simple.RabbitMQ
{
    public interface IRabbitProducer : ISingletonDependency
    {
        void Send<TMessageQueue>(TMessageQueue message) where TMessageQueue : IMessageQueue;
        void Send<TMessageQueue>(TMessageQueue message, string routingKey) where TMessageQueue : IMessageQueue;
    }
}
