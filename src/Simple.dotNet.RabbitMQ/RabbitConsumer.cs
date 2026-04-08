using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.RabbitMQ
{
    public class RabbitConsumer : RabbitConnection, IRabbitConsumer
    {

        private readonly IListenerMessage _listener;
        /// <summary>
        /// 最后一次ACK的时间
        /// </summary>
        private DateTime _lastAckAt;
        /// <summary>
        /// 多少秒未ACK自动触发重连机制
        /// </summary>
        private int _lastAckTimeoutRestart;

        private readonly ConsumerAttribute _consumer;
        public RabbitConsumer(IListenerMessage listener, ConsumerAttribute consumer)
        {
            _listener = listener;
            _lastAckAt = DateTime.Now;
            _lastAckTimeoutRestart = 60 * 5;
            _consumer = consumer;
        }
        public void Start()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(_consumer.ExchangeName, _consumer.Type, true, false, null);
                channel.QueueDeclare(queue: _consumer.QueueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(_consumer.QueueName, _consumer.ExchangeName, _consumer.RoutingKey ?? string.Empty, null);
                channel.BasicQos(0, _consumer.Unacked, false);
                var eventing = new EventingBasicConsumer(channel);
                eventing.Received += (s, t) =>
                {
                    string message = Encoding.Default.GetString(t.Body);
                    try
                    {
                        _lastAckAt = DateTime.Now;
                        //ConsoleHelper.WriteLine($"队列：{_consumer.QueueName} 时间：{DateTime.Now}", ConsoleColor.Green);
                        _listener.Invoke(message, s, t);
                        //Channel.BasicReject(t.DeliveryTag, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        _listener.Invoke(message, s, t);
                    }
                    finally
                    {
                        //手动发送确认消息
                        channel.BasicAck(t.DeliveryTag, false);
                    }
                };
                channel.BasicConsume(_consumer.QueueName, autoAck: false, consumer: eventing);
            }
        }
    }
}
