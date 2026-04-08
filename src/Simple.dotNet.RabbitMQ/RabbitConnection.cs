using RabbitMQ.Client;
using Simple.Core.Dependency;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// Rabbitmq连接基类
    /// </summary>
    public abstract class RabbitConnection
    {
        private readonly RabbitOption _options;
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;

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
    }
}
