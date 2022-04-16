using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase<TMessageQueue> : ConsumerBase where TMessageQueue : IMessageQueue
    {
        public abstract void Invoke(TMessageQueue message, object sender, BasicDeliverEventArgs args);
    }
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase : ConsumerBase
    {
        public abstract void Invoke(string message, object sender, BasicDeliverEventArgs args);
    }
    public abstract class ConsumerBase
    {
        public virtual void Faild(string error, object sender, BasicDeliverEventArgs ea)
        {

        }
    }
}
