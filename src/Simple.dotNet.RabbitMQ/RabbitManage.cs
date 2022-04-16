using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.Core.Extensions;
using System.Diagnostics;
using Simple.Core.Helper;
using RabbitMQ.Client.Events;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 生产
    /// </summary>
    public static class RabbitManage
    {
        private static RabbitConnection _connection;
        static RabbitManage()
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
            message.Send(string.Empty);
        }
        public static void Send<TMessageQueue>(this TMessageQueue message, string routingKey) where TMessageQueue : IMessageQueue
        {
            Stopwatch sw = Stopwatch.StartNew();
            ProducerAttribute exchange = typeof(TMessageQueue).GetAttribute<ProducerAttribute>();
            if (exchange == null) throw new RabbitException(nameof(ProducerAttribute));
            string msg = JsonConvert.SerializeObject(message);
            _connection.Channel.ExchangeDeclare(exchange.Name, exchange.Type, true, false, null);
            // 消息持久化
            var properties = _connection.Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(msg);
            //绑定交换机
            _connection.Channel.BasicPublish(exchange.Name, routingKey, properties, body);
            ConsoleHelper.WriteLine($"交换机：{exchange.Name}\n内容：{msg}\n 耗时：{sw.ElapsedMilliseconds}", ConsoleColor.Green);
        }
        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="action"></param>
        public static void Consumer(ConsumerAttribute consumer, Func<string, object, BasicDeliverEventArgs, bool> action)
        {
            _connection.Channel.ExchangeDeclare(consumer.ExchangeName, consumer.Type, true, false, null);
            _connection.Channel.QueueDeclare(queue: consumer.QueueName, durable: true, exclusive: false, autoDelete: false);
            _connection.Channel.QueueBind(consumer.QueueName, consumer.ExchangeName, consumer.RoutingKey ?? string.Empty, null);
            _connection.Channel.BasicQos(0, consumer.Unacked, false);
            var eventing = new EventingBasicConsumer(_connection.Channel);
            eventing.Received += (s, t) =>
            {
                string message = Encoding.Default.GetString(t.Body);
                bool ask = action(message, s, t);
                if (ask)
                {
                    //手动发送确认消息
                    _connection.Channel.BasicAck(t.DeliveryTag, false);
                }
                else
                {
                    _connection.Channel.BasicReject(t.DeliveryTag, true);
                }
            };
            _connection.Channel.BasicConsume(consumer.QueueName, false, eventing);
        }
    }
}
