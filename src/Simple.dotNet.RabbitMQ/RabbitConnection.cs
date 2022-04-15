using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Extensions;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// Rabbitmq连接基类
    /// </summary>
    public abstract class RabbitConnection
    {
        private readonly RabbitOption _options;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

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
            _channel = _connection.CreateModel();
        }

        protected IModel Channel
        {
            get
            {
                if (!_channel.IsOpen)
                {
                    return _connection.CreateModel();
                }
                return _channel;
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        protected void Close()
        {
            if (_channel != null)
            {
                _channel.Close();
            }
            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}
