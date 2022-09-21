using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 消费监听接口
    /// </summary>
    public interface IListenerMessage
    {
        void Invoke(string message, object sender, BasicDeliverEventArgs args);
        void Faild(string error, object sender, BasicDeliverEventArgs args);
    }
    public abstract class ListenerMessage<TMessageQueue> : IListenerMessage where TMessageQueue : IMessageQueue
    {
        public void Faild(string error, object sender, BasicDeliverEventArgs args)
        {

        }

        public void Invoke(string message, object sender, BasicDeliverEventArgs args)
        {
            if (string.IsNullOrEmpty(message)) return;
            TMessageQueue? model = JsonConvert.DeserializeObject<TMessageQueue>(message);
            if (model == null) return;
            this.Invoke(model);
        }
        public abstract void Invoke(TMessageQueue message);
    }
}
