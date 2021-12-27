using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Sqlite;
using Simple.dotNet.Core.Logger;
using Simple.dotNet.Core.Helper;
using System.Threading.Tasks;
using System.Threading;

namespace Simple.dotNet.Healthy
{
    public static class HealthyServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthy(this IServiceCollection services)
        {
            services.AddDepency();
            services.AddSqlite();
            return services;
        }
    }
}
