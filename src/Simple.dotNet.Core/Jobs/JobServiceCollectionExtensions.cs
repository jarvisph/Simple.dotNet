using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Helper;
using Simple.Core.Extensions;

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
        public static IServiceCollection AddJob(this IServiceCollection services, string[] args = null)
        {
            foreach (var assemblie in AssemblyHelper.GetAssemblies())
            {
                IEnumerable<Type> types = assemblie.GetTypes().Where(t => typeof(JobServiceBase).IsAssignableFrom(t)).Where(c => c.IsPublic && !c.IsAbstract);
                if (args != null)
                {
                    string jobs = args.Get("-job");
                    if (!string.IsNullOrWhiteSpace(jobs))
                    {
                        string[] tasks = jobs.Split(',');
                        types = types.Where(c => tasks.Contains(c.Name));
                    }
                }
                foreach (Type type in types)
                {
                    Task.Run(() =>
                    {
                        Console.WriteLine($"==============已启动{type.Name}任务=================");
                        JobServiceBase service = (JobServiceBase)Activator.CreateInstance(type);
                        if (service != null)
                        {
                            service.Start();
                        }
                    });
                }
            }
            return services;
        }
    }
}
