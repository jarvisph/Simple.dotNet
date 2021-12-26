using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Dependency;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 生产者
    /// </summary>
    public interface IRabbitProducer
    {
        void Send(string message, string exchange);
        void Send<TMessageQueue>(TMessageQueue message, string exchange) where TMessageQueue : IMessageQueue;
    }
}
