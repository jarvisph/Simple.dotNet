using Microsoft.Extensions.DependencyInjection;

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
            return services;
        }
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string connection)
        {
            RabbitOption options = new RabbitOption(connection);
            services.AddRabbitMQ(options);
            services.AddSingleton<IMessageProducer, NewtonsoftProducer>();
            return services;
        }
    }
}
