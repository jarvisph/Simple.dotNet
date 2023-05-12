using Newtonsoft.Json;
using RabbitMQ.Client;
using Simple.Core.Extensions;
using Simple.Core.Helper;
using System;
using System.Text;

namespace Simple.RabbitMQ
{
    public class RabbitProducer : RabbitConnection, IRabbitProducer
    {
        public void Send<TMessageQueue>(TMessageQueue message) where TMessageQueue : IMessageQueue
        {
            Send(message, string.Empty);
        }

        public void Send<TMessageQueue>(TMessageQueue message, string routingKey) where TMessageQueue : IMessageQueue
        {
            Open();
            ProducerAttribute exchange = typeof(TMessageQueue).GetAttribute<ProducerAttribute>();
            if (exchange == null) throw new RabbitException(nameof(ProducerAttribute));
            string msg = JsonConvert.SerializeObject(message);
            _channel.ExchangeDeclare(exchange.Name, exchange.Type, true, false, null);
            // 消息持久化
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(msg);
            //绑定交换机
            _channel.BasicPublish(exchange.Name, routingKey, properties, body);
            ConsoleHelper.WriteLine($"交换机：{exchange.Name} 时间：{DateTime.Now}", ConsoleColor.Green);
        }
    }
}
