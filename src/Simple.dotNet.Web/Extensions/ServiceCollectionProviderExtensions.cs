using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Dependency;
using Microsoft.AspNetCore.Http;
using Simple.dotNet.Core.Domain.Dto;

namespace Simple.dotNet.Web.Extensions
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
            //作用域注入
            services.AddScoped<ErrorMessageResult>();//错误消息返回
            return services.AddDepency();
        }
    }
}
