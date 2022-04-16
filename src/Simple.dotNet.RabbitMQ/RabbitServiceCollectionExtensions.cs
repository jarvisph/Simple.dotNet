using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Helper;
using System.Diagnostics;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// rabbit 注册类
    /// </summary>
    public static class RabbitServiceCollectionExtensions
    {
        /// <summary>
        /// 注册rabbit生产
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitProduce(this IServiceCollection services, string connectionString)
        {
            services.AddRabbitConnection(connectionString);
            return services;
        }

        /// <summary>
        /// 注册rabbit消费
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConsumer(this IServiceCollection services)
        {
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                Parallel.ForEach(assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && t.BaseType == typeof(RabbitConsumerBase)), type =>
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    ConsumerAttribute consumer = type.GetCustomAttribute<ConsumerAttribute>();
                    if (consumer == null)
                    {
                        ConsoleHelper.WriteLine($"未标记：{nameof(ConsumerAttribute)}", ConsoleColor.Red);
                    }
                    else if (string.IsNullOrEmpty(consumer.ExchangeName))
                    {
                        ConsoleHelper.WriteLine($"交换器为空", ConsoleColor.Red);
                    }
                    else
                    {
                        RabbitConsumerBase service = (RabbitConsumerBase)Activator.CreateInstance(type);
                        RabbitConsumer.Consumer(consumer, (message, sender, args) =>
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
        /// <summary>
        /// 注册rabbit连接
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitConnection(this IServiceCollection services, string connection)
        {
            return services;
        }
    }
}
