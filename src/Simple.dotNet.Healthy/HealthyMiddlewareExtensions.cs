using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Helper;
using Simple.dotNet.Core.Logger;
using Simple.dotNet.Healthy.Entity;
using Simple.dotNet.Healthy.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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
