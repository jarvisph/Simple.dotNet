using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// Rabbit 消费
    /// </summary>
    public interface IRabbitConsumer
    {
        void Consumer(string exchange, string queue, string type, Func<string, object, BasicDeliverEventArgs, bool> action);
    }
}
