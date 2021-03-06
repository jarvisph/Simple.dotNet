using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Simple.Core.Dependency;
using Simple.Core.Extensions;
using Simple.Core.Localization;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// Rabbitmq连接基类
    /// </summary>
    public class RabbitConnection
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

        public IModel Channel
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
        public void Close()
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
