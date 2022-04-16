using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Helper;

namespace Simple.Core.Jobs
{
    /// <summary>
    /// job定时服务
    /// </summary>
    public static class JobServiceCollectionExtensions
    {
        /// <summary>
        /// 注入定时任务
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddJob(this IServiceCollection services)
        {
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                IEnumerable<Type> types = assemblie.GetTypes().Where(t => t.IsPublic && !t.IsAbstract && t.BaseType == typeof(JobServiceBase));
                Parallel.ForEach(types, type =>
                {
                    Console.WriteLine($"==============已启动{type.Name}任务=================");
                    JobServiceBase service = (JobServiceBase)Activator.CreateInstance(type);
                    if (service != null)
                    {
                        service.Start();
                    }
                });
            }
            return services;
        }
    }
}
