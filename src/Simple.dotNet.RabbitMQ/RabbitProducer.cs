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
        private static RabbitConnection _connection;
        static RabbitProducer()
        {
            _connection = new RabbitConnection();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMessageQueue"></typeparam>
        /// <param name="message"></param>
        /// <exception cref="RabbitException"></exception>
        public static void Send<TMessageQueue>(this TMessageQueue message) where TMessageQueue : IMessageQueue
        {
            ProducerAttribute exchange = typeof(TMessageQueue).GetAttribute<ProducerAttribute>();
            if (exchange == null) throw new RabbitException(nameof(ProducerAttribute));
            string msg = JsonConvert.SerializeObject(message);
            string routingKey = string.Empty;
            _connection.Channel.ExchangeDeclare(exchange.Name, exchange.Type, true, false, null);
            // 消息持久化
            var properties = _connection.Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(msg);
            //绑定交换机
            _connection.Channel.BasicPublish(exchange.Name, routingKey, properties, body);
        }
    }
}
