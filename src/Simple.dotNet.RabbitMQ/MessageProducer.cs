using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.RabbitMQ
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(string queueName, T message);
        Task PublishBatchAsync<T>(string queueName, List<T> messages);
    }
    public class NewtonsoftProducer : IMessageProducer, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly SemaphoreSlim _channelSemaphore;
        private readonly List<IChannel> _channels;
        private readonly Random _random = new();
        private bool _disposed;

        public NewtonsoftProducer(IConnection connection)
        {
            _connection = connection;
            _channelSemaphore = new SemaphoreSlim(10, 10);
            _channels = new List<IChannel>();


            InitializeChannels().GetAwaiter().GetResult();
        }

        private async Task InitializeChannels()
        {
            for (int i = 0; i < 10; i++)
            {
                var channel = await _connection.CreateChannelAsync();
                await channel.BasicQosAsync(0, 100, false);
                _channels.Add(channel);
            }
        }

        private IChannel GetAvailableChannel()
        {
            return _channels[_random.Next(_channels.Count)];
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            await _channelSemaphore.WaitAsync();
            var channel = GetAvailableChannel();

            try
            {
                // 声明队列
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                // 使用 JsonConvert.SerializeObject
                var jsonStr = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonStr);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                // 添加自定义头
                properties.Headers = new Dictionary<string, object?>
                {
                    ["x-message-type"] = typeof(T).Name,
                    ["x-serialize-type"] = "Newtonsoft.Json"
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);
            }
            finally
            {
                _channelSemaphore.Release();
            }
        }

        public async Task PublishBatchAsync<T>(string queueName, List<T> messages)
        {
            if (messages.Count == 0) return;

            await _channelSemaphore.WaitAsync();
            var channel = GetAvailableChannel();

            try
            {
                await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

                // 方式1：逐条发送（简单但性能较低）
                foreach (var message in messages)
                {
                    var jsonStr = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(jsonStr);

                    var properties = new BasicProperties
                    {
                        ContentType = "application/json",
                        DeliveryMode = DeliveryModes.Persistent
                    };

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: false,
                        basicProperties: properties,
                        body: body);
                }
            }
            finally
            {
                _channelSemaphore.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            foreach (var channel in _channels)
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }

            _channelSemaphore.Dispose();
            _disposed = true;
        }
    }
}
