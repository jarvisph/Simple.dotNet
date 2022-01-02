using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Sqlite;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Healthy.DataContext;
using Microsoft.EntityFrameworkCore;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.Healthy
{
    public static class HealthyServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthy(this IServiceCollection services)
        {
            services.AddSqlite();
            services.AddDepency();
            services.AddDbContext<HealthyDbContext>(opt => opt.UseSqlite(AppsettingConfig.GetConnectionString("DbConnection")));
            return services;
        }
    }
}
