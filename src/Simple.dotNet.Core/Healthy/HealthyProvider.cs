using Simple.dotNet.Core.Helper;
using Simple.dotNet.Core.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Healthy
{
    internal static class HealthyProvider
    {
        public static void Run(HealthyOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ServiceName)) throw new MessageException("servicename not null");
            if (string.IsNullOrWhiteSpace(options.Host)) throw new MessageException("host not null");
            //定义客户端ID
            string clientId = Guid.NewGuid().ToString();
            Task.Run(() =>
            {
                try
                {
                    //注册服务
                    string reuslt = NetHelper.Post($"{options.Address}/healthy/register", new Dictionary<string, object> {
                        { "ClientID", clientId },
                        { "ServiceName", options.ServiceName },
                        { "Host", options.Host },
                        { "Port", options.Port?? 0},
                        { "HostName",Dns.GetHostName() },
                        { "Tags",options.Tags??string.Empty },
                        { "HealthCheck", options.HealthCheck??string.Empty }
                     });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
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
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                });
            }
        }
    }
}
