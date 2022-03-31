using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 生产类
    /// </summary>
    public static class RabbitProducer
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="exchange">交换机</param>
        public static void Send(string message, string exchange)
        {
            //string routingKey = string.Empty;

            //Channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
            //// 消息持久化
            //var properties = Channel.CreateBasicProperties();
            //properties.Persistent = true;
            //properties.DeliveryMode = 2;

            //var body = Encoding.UTF8.GetBytes(message);

            ////绑定交换机
            //Channel.BasicPublish(exchange, routingKey, properties, body);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMessageQueue"></typeparam>
        /// <param name="message"></param>
        /// <exception cref="RabbitException"></exception>
        public static void Send<TMessageQueue>(this TMessageQueue message) where TMessageQueue : IMessageQueue
        {
            ExchangeAttribute exchange = typeof(TMessageQueue).GetAttribute<ExchangeAttribute>();
            if (exchange == null) throw new RabbitException(nameof(ExchangeAttribute));
            string msg = JsonConvert.SerializeObject(message);
        }
    }
}
