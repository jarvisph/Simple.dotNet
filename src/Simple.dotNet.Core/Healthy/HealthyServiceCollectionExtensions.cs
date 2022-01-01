using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.Core.Healthy
{
    public static class HealthyServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthy(this IServiceCollection services)
        {
            HealthyOptions options = AppsettingConfig.GetConfig<HealthyOptions>("Healthy");
            return services.AddHealthy(options);
        }
        /// <summary>
        /// 注册健康检查组件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHealthy(this IServiceCollection services, HealthyOptions options)
        {
            HealthyProvider.Run(options);
            return services;
        }
    }
}
