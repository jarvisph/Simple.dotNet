using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dependency;
using Microsoft.AspNetCore.Http;
using Simple.Core.Domain.Dto;

namespace Simple.Web.Extensions
{
    public static class ServiceCollectionProviderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSimple(this IServiceCollection services)
        {
            services.AddOptions();
            //单例注入
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services.AddDepency();
        }
    }
}
