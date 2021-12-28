using Microsoft.AspNetCore.Builder;

namespace Simple.dotNet.Core.Healthy
{
    public static class HealthyMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthy(this IApplicationBuilder app, HealthyOptions options)
        {
            HealthyProvider.Run(options);
            return app;
        }
    }
}
