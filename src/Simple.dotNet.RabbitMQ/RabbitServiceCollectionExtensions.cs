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
            services.AddSingleton<IRabbitConsumer, RabbitConsumer>();
            IRabbitConsumer consumer = IocCollection.Resolve<IRabbitConsumer>();
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                IEnumerable<Type> types = assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(RabbitConsumerBase<>));
                Parallel.ForEach(types, type =>
                {
                    object obj = Activator.CreateInstance(type);
                    var exchange = (string)type.GetProperty("Exchange").GetValue(obj);
                    var queue = (string)type.GetProperty("Queue").GetValue(obj);
                    var serverType = (string)type.GetProperty("Type").GetValue(obj);
                    var generics = type.BaseType.GetGenericArguments();
                    consumer.Consumer(exchange, queue, serverType, (message, sender, args) =>
                    {
                        var param = new object[] { JsonConvert.DeserializeObject(message, generics[0]), sender, args };
                        MethodInfo method = type.GetMethod("Invoke");
                        return (bool)method.Invoke(obj, param);
                    });
                });

                Parallel.ForEach(assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && t.BaseType == typeof(RabbitConsumerBase)), type =>
                {
                    RabbitConsumerBase service = (RabbitConsumerBase)Activator.CreateInstance(type);
                    consumer.Consumer(service.Exchange, service.Queue, service.Type, (message, sender, args) =>
                    {
                        return service.Invoke(message, sender, args);
                    });
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
