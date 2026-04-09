using RabbitMQ.Client;
using Simple.Core.Dependency;
using System;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// Rabbitmq连接基类
    /// </summary>
    public abstract class RabbitConnection
    {
        protected readonly RabbitOption _options;
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected IModel _channel;

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
            _connection = _factory.CreateConnection();

        }
        public void Open()
        {
            if (_channel == null || !_channel.IsOpen) _channel = _connection.CreateModel();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                if (_channel != null)
                {
                    _channel.Close();
                    _channel.Dispose();
                    _channel = null;
                }

            }
            catch
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                    _channel = null;
                }
            }

        }
    }
}
