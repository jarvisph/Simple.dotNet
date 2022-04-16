using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 监听rabbit消费基类
    /// </summary>
    public static class RabbitConsumer
    {
        private static RabbitConnection _connection;
        static RabbitConsumer()
        {
            _connection = new RabbitConnection();
        }
        public static void Consumer(ConsumerAttribute consumer, Func<string, object, BasicDeliverEventArgs, bool> action)
        {
            _connection.Channel.ExchangeDeclare(consumer.ExchangeName, consumer.Type, true, false, null);
            _connection.Channel.QueueDeclare(queue: consumer.QueueName, durable: true, exclusive: false, autoDelete: false);
            _connection.Channel.QueueBind(consumer.QueueName, consumer.ExchangeName, string.Empty, null);
            //_connection.Channel.BasicQos(1);
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
