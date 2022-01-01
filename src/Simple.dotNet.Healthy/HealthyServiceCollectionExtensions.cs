using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Sqlite;
using Simple.dotNet.Core.Dependency;

namespace Simple.dotNet.Healthy
{
    public static class HealthyServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthy(this IServiceCollection services)
        {
            services.AddSqlite();
            services.AddDepency();
            return services;
        }
    }
}
