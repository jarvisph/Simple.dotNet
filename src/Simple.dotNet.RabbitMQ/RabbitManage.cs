using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Simple.Core.Extensions;
using System.Diagnostics;
using Simple.Core.Helper;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;
using Simple.Core.Dependency;
using System.Linq;
using System.Reflection;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 生产
    /// </summary>
    public static class RabbitManage
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMessageQueue"></typeparam>
        /// <param name="message"></param>
        /// <exception cref="RabbitException"></exception>
        public static void Send<TMessageQueue>(this TMessageQueue message) where TMessageQueue : IMessageQueue
        {
            IRabbitProducer producer = IocCollection.Resolve<IRabbitProducer>();
            producer.Send(message);
        }
        public static void Send<TMessageQueue>(this TMessageQueue message, string routingKey) where TMessageQueue : IMessageQueue
        {
            IRabbitProducer producer = IocCollection.Resolve<IRabbitProducer>();
            producer.Send(message, routingKey);
        }
        public static void Consumer()
        {
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                Parallel.ForEach(assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(IListenerMessage).IsAssignableFrom(t)), type =>
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    ConsumerAttribute? consumer = type.GetCustomAttribute<ConsumerAttribute>();
                    if (consumer == null)
                    {
                        ConsoleHelper.WriteLine($"消费：{type.Name}，未标记：{nameof(ConsumerAttribute)}", ConsoleColor.Red);
                    }
                    else if (string.IsNullOrEmpty(consumer.ExchangeName))
                    {
                        ConsoleHelper.WriteLine($"消费：{type.Name}，交换器为空", ConsoleColor.Red);
                    }
                    else
                    {
                        IListenerMessage? service = (IListenerMessage?)Activator.CreateInstance(type);
                        if (service == null) return;
                        IRabbitConsumer _rabbit = new RabbitConsumer(service, consumer);
                        _rabbit.Start();
                        Console.WriteLine($"消费：{type.Name}，已启动，耗时：{sw.ElapsedMilliseconds}ms");
                    }
                });

            }
        }
    }
}
