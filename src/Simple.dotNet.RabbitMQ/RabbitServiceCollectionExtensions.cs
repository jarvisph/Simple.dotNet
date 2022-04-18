using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// rabbitmq 注册类
    /// </summary>
    public static class RabbitServiceCollectionExtensions
    {
        /// <summary>
        /// 注册rabbit消费
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, RabbitOption options)
        {
            services.AddSingleton(c => options);
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                Parallel.ForEach(assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && typeof(RabbitConsumerBase).IsAssignableFrom(t)), type =>
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
                        RabbitConsumerBase? service = (RabbitConsumerBase?)Activator.CreateInstance(type);
                        if (service == null) return;
                        RabbitManage.Consumer(consumer, (message, sender, args) =>
                        {
                            service.Invoke(message, sender, args);
                            return true;
                        });
                        Console.WriteLine($"消费：{type.Name}，已启动，耗时：{sw.ElapsedMilliseconds}ms");
                    }
                });

            }
            return services;
        }
    }
}
