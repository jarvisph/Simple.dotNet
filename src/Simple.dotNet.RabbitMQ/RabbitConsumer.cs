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
        private readonly ConsumerAttribute _consumer;
        private DateTime _lastAckAt;
        private readonly int _lastAckTimeoutRestart;
        private CancellationTokenSource? _healthCheckCts;
        private Task? _healthCheckTask;
        private bool _isConnecting;

        public RabbitConsumer(IListenerMessage listener, ConsumerAttribute consumer)
        {
            _listener = listener;
            _consumer = consumer;
            _lastAckAt = DateTime.Now;
            _lastAckTimeoutRestart = 60 * 5; // 5分钟
        }
        /// <summary>
        /// 打开通道
        /// </summary>
        public async Task OpenAsync()
        {
            if (_channel == null || _channel.IsOpen == false)
            {
                _channel = await _connection!.CreateChannelAsync();
                Console.WriteLine($"通道已打开，队列: {_consumer.QueueName}");
            }
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        public async Task CloseAsync()
        {
            try
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                    await _channel.DisposeAsync();
                    _channel = null;
                    Console.WriteLine($"通道已关闭，队列: {_consumer.QueueName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _channel?.Dispose();
                _channel = null;
            }
        }

        /// <summary>
        /// 连接并声明交换机和队列
        /// </summary>
        public async Task ConnectAsync()
        {
            if (_isConnecting)
            {
                Console.WriteLine("正在连接中，跳过重复连接请求");
                return;
            }

            try
            {
                _isConnecting = true;
                await OpenAsync();

                if (_channel == null)
                    throw new InvalidOperationException("通道未初始化");

                // 声明交换机（持久化）
                await _channel.ExchangeDeclareAsync(
                    exchange: _consumer.ExchangeName,
                    type: _consumer.Type,
                    durable: true,
                    autoDelete: false);

                // 声明队列（持久化）
                await _channel.QueueDeclareAsync(
                    queue: _consumer.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                // 绑定队列到交换机
                await _channel.QueueBindAsync(
                    queue: _consumer.QueueName,
                    exchange: _consumer.ExchangeName,
                    routingKey: _consumer.RoutingKey ?? string.Empty);

                // 设置 QoS（每次预取指定数量的消息）
                await _channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: _consumer.Unacked,
                    global: false);

                // 创建消费者
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += OnMessageReceivedAsync;

                // 开始消费（手动确认模式）
                await _channel.BasicConsumeAsync(
                    queue: _consumer.QueueName,
                    autoAck: false,
                    consumer: consumer);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                _isConnecting = false;
            }
        }

        /// <summary>
        /// 消息接收处理器（异步版本）
        /// </summary>
        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            string message = Encoding.UTF8.GetString(args.Body.ToArray());

            try
            {
                _lastAckAt = DateTime.Now;

                // 调用业务处理逻辑
                await _listener.Invoke(message, sender, args);

                // 手动发送确认消息
                if (_channel != null && _channel.IsOpen)
                {
                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                try
                {
                    // 处理失败，根据配置决定是否重新入队
                    if (_channel != null && _channel.IsOpen)
                    {
                        // 可以在这里实现重试逻辑
                        await _channel.BasicNackAsync(args.DeliveryTag, false, requeue: true);
                    }
                }
                catch (Exception nackEx)
                {
                    Console.WriteLine(nackEx);
                }
            }
        }

        /// <summary>
        /// 启动消费者（异步版本）
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await ConnectAsync();
            StartHealthCheck(cancellationToken);
        }

        /// <summary>
        /// 启动消费者（同步版本，保持向后兼容）
        /// </summary>
        public void Start()
        {
            // 同步调用异步方法
            Task.Run(async () => await StartAsync()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 重启消费者
        /// </summary>
        private async Task ReStartAsync()
        {
            Console.WriteLine($"开始重启消费者 - 队列: {_consumer.QueueName}");
            await CloseAsync();
            _lastAckAt = DateTime.Now;
            await ConnectAsync();
            Console.WriteLine($"消费者重启完成 - 队列: {_consumer.QueueName}");
        }

        /// <summary>
        /// 启动健康检查
        /// </summary>
        private void StartHealthCheck(CancellationToken cancellationToken)
        {
            _healthCheckCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _healthCheckTask = Task.Run(async () =>
            {
                while (!_healthCheckCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(3000, _healthCheckCts.Token);

                        // 检查连接状态
                        if (_channel == null || _channel.IsOpen == false)
                        {
                            Console.WriteLine($"检测到连接已关闭，开始重连 - 队列: {_consumer.QueueName}");
                            await ReStartAsync();
                        }
                        else if ((DateTime.Now - _lastAckAt).TotalSeconds >= _lastAckTimeoutRestart)
                        {
                            Console.WriteLine($"距上一次消费已过 {(DateTime.Now - _lastAckAt).TotalSeconds} 秒，无新消息，尝试重连 - 队列: {_consumer.QueueName}");
                            await ReStartAsync();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }, _healthCheckCts.Token);
        }

        /// <summary>
        /// 停止健康检查
        /// </summary>
        private async Task StopHealthCheckAsync()
        {
            if (_healthCheckCts != null)
            {
                await _healthCheckCts.CancelAsync();
                _healthCheckCts.Dispose();
                _healthCheckCts = null;
            }

            if (_healthCheckTask != null)
            {
                try
                {
                    await _healthCheckTask;
                }
                catch (OperationCanceledException)
                {
                    // 预期异常，忽略
                }
                _healthCheckTask = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override async ValueTask DisposeAsync()
        {
            await StopHealthCheckAsync();
            await CloseAsync();
            await base.DisposeAsync();
        }
    }
}
