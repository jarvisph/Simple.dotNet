using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase<TMessageQueue> where TMessageQueue : IMessageQueue
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public abstract string Queue { get; set; }
        /// <summary>
        /// 交换机名称
        /// </summary>
        public abstract string Exchange { get; set; }
        /// <summary>
        /// 默认启动
        /// </summary>
        public virtual bool Enable { get; set; } = true;
        /// <summary>
        /// exchange type ExchangeType
        /// </summary>
        public virtual string Type { get; set; } = ExchangeType.Direct;
        /// <summary>
        /// 消息数量
        /// </summary>
        public virtual int Unacked { get; set; } = 8;
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        public abstract void Invoke(TMessageQueue message, object sender, BasicDeliverEventArgs args);

        /// <summary>
        /// 错误异常
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        public virtual void Faild(string error, TMessageQueue message, object sender, BasicDeliverEventArgs ea)
        {

        }
    }
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase
    {
        public abstract string Queue { get; set; }
        public abstract string Exchange { get; set; }
        public virtual string Type { get; set; } = ExchangeType.Direct;

        public abstract bool Invoke(string message, object sender, BasicDeliverEventArgs args);
        public virtual void Faild(string error, object sender, BasicDeliverEventArgs ea)
        {

        }
    }
}
