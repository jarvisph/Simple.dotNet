using RabbitMQ.Client.Events;


namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase<TMessageQueue> where TMessageQueue : IMessageQueue
    {
        public abstract string Queue { get; set; }
        public abstract string Exchange { get; set; }
        public abstract string Type { get; set; }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        public abstract bool Invoke(TMessageQueue message, object sender, BasicDeliverEventArgs args);

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
        public abstract string Type { get; set; }

        public abstract bool Invoke(string message, object sender, BasicDeliverEventArgs args);
        public virtual void Faild(string error, object sender, BasicDeliverEventArgs ea)
        {

        }
    }
}
