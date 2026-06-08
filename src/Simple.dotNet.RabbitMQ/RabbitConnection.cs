using RabbitMQ.Client;
using Simple.Core.Dependency;
using System;
using System.Threading.Tasks;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// Rabbitmq连接基类
    /// </summary>
    public abstract class RabbitConnection : IAsyncDisposable
    {
        protected readonly RabbitOption _options;
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected IChannel? _channel;
        private bool _disposed;
        public RabbitConnection()
        {
            _options = IocCollection.Resolve<RabbitOption>();
            _factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                Port = _options.Port,
                AutomaticRecoveryEnabled = true
            };
            _connection = _factory.CreateConnectionAsync().Result;
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
