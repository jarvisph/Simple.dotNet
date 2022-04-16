using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Simple.RabbitMQ
{
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase<TMessageQueue> : RabbitConsumerBase where TMessageQueue : IMessageQueue
    {
        public abstract void Invoke(TMessageQueue message, object sender, BasicDeliverEventArgs args);
        public override void Invoke(string message, object sender, BasicDeliverEventArgs args)
        {
            TMessageQueue queue = JsonConvert.DeserializeObject<TMessageQueue>(message);
            if (queue == null) return;
            Invoke(queue, sender, args);
        }
    }
    /// <summary>
    /// 消费服务基类
    /// </summary>
    public abstract class RabbitConsumerBase
    {
        public abstract void Invoke(string message, object sender, BasicDeliverEventArgs args);
        public virtual void Faild(string error, object sender, BasicDeliverEventArgs ea)
        {

        }
    }
}
