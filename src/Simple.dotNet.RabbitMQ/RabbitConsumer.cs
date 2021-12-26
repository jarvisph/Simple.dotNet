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
    public class RabbitConsumer : RabbitConnection, IRabbitConsumer
    {
        public void Consumer(string exchange, string queue, string type, Func<string, object, BasicDeliverEventArgs, bool> action)
        {
            Channel.ExchangeDeclare(exchange, type, true, false, null);
            Channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
            Channel.QueueBind(queue, exchange, "", null);
            //Channel.BasicQos(1);
            var consumer = new EventingBasicConsumer(this.Channel);
            consumer.Received += (s, t) =>
            {
                string message = Encoding.Default.GetString(t.Body);
                bool ask = action(message, s, t);
                if (ask)
                {
                    //手动发送确认消息
                    Channel.BasicAck(t.DeliveryTag, false);
                }
                else
                {
                    Channel.BasicReject(t.DeliveryTag, true);
                }

            };
            Channel.BasicConsume(queue, false, consumer);
        }
    }
}
