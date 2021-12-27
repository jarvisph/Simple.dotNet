using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Helper;
using Simple.dotNet.Core.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Healthy
{
    public static class HealthyServiceCollectionExtensions
    {
        /// <summary>
        /// 注册健康检查组件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHealthy(this IServiceCollection services, HealthyOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ServiceName)) throw new MessageException("servicename not null");
            if (string.IsNullOrWhiteSpace(options.Host)) throw new MessageException("host not null");
            //定义客户端ID
            string clientId = Guid.NewGuid().ToString();
            try
            {
                //注册服务
                string reuslt = NetHelper.Post($"{options.Address}/healthy/register", new Dictionary<string, object> {
                { "ClientID", clientId },
                { "ServiceName", options.ServiceName },
                { "Host", options.Host },
                { "Port", options.Port?? 0},
                { "HealthCheck", options.HealthCheck??string.Empty }
            });
            }
            catch (Exception)
            {
                throw;
            }
         
            if (!options.Port.HasValue)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {

                            NetHelper.Post($"{options.Address}/healthy/check", new Dictionary<string, object> {
                                        { "ClientID", clientId },
                               });
                            Thread.Sleep(options.Interval.HasValue ? options.Interval.Value.Milliseconds : 1000 * 5);
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }
            return services;
        }
    }
}
