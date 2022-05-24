using Microsoft.Extensions.DependencyInjection;
using System;

namespace Simple.Redis
{
    /// <summary>
    /// 注册Redis
    /// </summary>
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(c => new RedisConnection(connectionString));
            return services;
        }
    }
}
