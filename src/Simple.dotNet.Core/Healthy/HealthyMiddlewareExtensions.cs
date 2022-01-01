using Microsoft.AspNetCore.Builder;
using Simple.dotNet.Core.Localization;
using System;

namespace Simple.dotNet.Core.Healthy
{
    public static class HealthyMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthy(this IApplicationBuilder app)
        {
            HealthyOptions options = AppsettingConfig.GetConfig<HealthyOptions>("Healthy");
            return app.UseHealthy(options);
        }
        public static IApplicationBuilder UseHealthy(this IApplicationBuilder app, HealthyOptions options)
        {
            options.ServiceName ??= AppsettingConfig.ServiceName;
            HealthyProvider.Run(options);
            return app;
        }
        public static IApplicationBuilder UseHealthy(this IApplicationBuilder app, Func<HealthyOptions> action)
        {
            HealthyOptions options = action();
            return app.UseHealthy(options);
        }
    }
}
