using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Simple.Core.Helper;
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
        public void Connect()
        {

        }
        public void Start()
        {
            Connect();
            CheckConsumerConnect();
        }
        private void ReStart()
        {
            _lastAckAt = DateTime.Now;
            Connect();
        }
        /// <summary>
        /// 检查消费连接状态
        /// </summary>
        private void CheckConsumerConnect()
        {
            // 检查连接状态
            CancellationTokenSource _cts = new CancellationTokenSource();
            Task.Factory.StartNew(token =>
            {
                var cancellationToken = (CancellationToken)token;
                while (true)
                {
                    try
                    {
                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            //// 未打开、关闭状态、上一次ACK超时，则重启
                            //if (_channel == null || _channel.IsClosed)
                            //{
                            //    Console.WriteLine($"队列：{_consumer.QueueName} \n时间：{DateTime.Now}\n内容：RabbitMQ连接已关闭，开始重新连接");
                            //    ReStart();
                            //}
                            //else if ((DateTime.Now - _lastAckAt).TotalSeconds >= _lastAckTimeoutRestart)
                            //{
                            //    Console.WriteLine($"队列：{_consumer.QueueName} \n时间：{DateTime.Now}\n内容：Rabbit距上一次消费过去了{(DateTime.Now - _lastAckAt).TotalSeconds}秒后没有新的消息，尝试重新连接Rabbit。");
                            //    ReStart();
                            //}
                            Thread.Sleep(3000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }, _cts.Token);
        }
    }
}
