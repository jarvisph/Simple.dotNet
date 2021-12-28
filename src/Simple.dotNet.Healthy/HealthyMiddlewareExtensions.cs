using Microsoft.AspNetCore.Builder;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Healthy.Entity;
using Simple.dotNet.Healthy.Services;
using System;
using System.Threading.Tasks;

namespace Simple.dotNet.Healthy
{
    public static class HealthyMiddlewareExtensions
    {
        /// <summary>
        /// 服务端
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthy(this IApplicationBuilder app, Action<HealthExamination> action)
        {
            IHealthyAppService service = IocCollection.Resolve<IHealthyAppService>();
            Task.Run(() =>
            {
                service.Check((item) =>
                {
                    action(item);
                });
            });
            return app;
        }
      
    }
}
