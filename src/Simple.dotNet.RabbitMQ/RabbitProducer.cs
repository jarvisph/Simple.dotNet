using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 生产基类
    /// </summary>
    public class RabbitProducer : RabbitConnection, IRabbitProducer
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="exchange">交换机</param>
        public void Send(string message, string exchange)
        {
            string routingKey = string.Empty;

            Channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
            // 消息持久化
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            var body = Encoding.UTF8.GetBytes(message);

            //绑定交换机
            Channel.BasicPublish(exchange, routingKey, properties, body);
        }

        public void Send<TMessageQueue>(TMessageQueue message, string exchange) where TMessageQueue : IMessageQueue
        {
            this.Send(JsonConvert.SerializeObject(message), exchange);
        }
    }
}
