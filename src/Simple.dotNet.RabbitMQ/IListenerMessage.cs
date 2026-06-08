using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Simple.Core.Dependency;
using Simple.Core.Helper;
using Simple.Core.Logger;
using System;
using System.Threading.Tasks;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 消费监听接口
    /// </summary>
    public interface IListenerMessage
    {
        Task Invoke(string message, object sender, BasicDeliverEventArgs args);
        void Faild(string error, object sender, BasicDeliverEventArgs args);
    }
    public abstract class ListenerMessage<TMessageQueue> : IListenerMessage where TMessageQueue : IMessageQueue
    {
        protected ILogger Logger { get; }
        public ListenerMessage()
        {
            Logger = IocCollection.Resolve<ILogger>();
        }
        public void Faild(string error, object sender, BasicDeliverEventArgs args)
        {

        }

        public Task Invoke(string message, object sender, BasicDeliverEventArgs args)
        {
            try
            {

                if (string.IsNullOrEmpty(message)) return Task.CompletedTask;
                TMessageQueue? model = JsonConvert.DeserializeObject<TMessageQueue>(message);
                if (model == null) return Task.CompletedTask;
                return this.Invoke(model);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ConsoleHelper.WriteLine(message, ConsoleColor.Red);
                return Task.CompletedTask;
            }
        }
        public abstract Task Invoke(TMessageQueue message);
    }
}
