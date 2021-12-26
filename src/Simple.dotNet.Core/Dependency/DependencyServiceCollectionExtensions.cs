using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.dotNet.Core.Helper;

namespace Simple.dotNet.Core.Dependency
{
    public static class DependencyServiceCollectionExtensions
    {
        /// <summary>
        /// Ioc容器注册 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDepency(this IServiceCollection services)
        {
            //遍历注入继承接口类
            foreach (Assembly assembly in AssemblyHelper.GetAssemblies())
            {
                IEnumerable<Type> transients = assembly.GetTypes().Where(t => typeof(ITransientDependency).IsAssignableFrom(t)).Where(t => !t.IsAbstract && t.IsClass);
                IEnumerable<Type> singletons = assembly.GetTypes().Where(t => typeof(ISingletonDependency).IsAssignableFrom(t)).Where(t => !t.IsAbstract && t.IsClass);
                IEnumerable<Type> scopeds = assembly.GetTypes().Where(t => typeof(IScopedDependency).IsAssignableFrom(t)).Where(t => !t.IsAbstract && t.IsClass);
                services.AddDepency(transients, ServiceLifetime.Transient);
                services.AddDepency(singletons, ServiceLifetime.Singleton);
                services.AddDepency(scopeds, ServiceLifetime.Scoped);
            }
            services.AddCollection();
            return services;
        }
        private static IServiceCollection AddDepency(this IServiceCollection services, IEnumerable<Type> types, ServiceLifetime lifetime)
        {
            foreach (var item in types)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (item.GetInterfaces().Length == 1)
                        {
                            services.AddSingleton(item);
                        }
                        else
                        {
                            foreach (var interfaceitem in item.GetInterfaces())
                            {
                                services.AddSingleton(interfaceitem, item);
                            }
                        }
                        break;
                    case ServiceLifetime.Scoped:
                        if (item.GetInterfaces().Length == 1)
                        {
                            services.AddSingleton(item);
                        }
                        else
                        {
                            foreach (var interfaceitem in item.GetInterfaces())
                            {
                                services.AddScoped(interfaceitem, item);
                            }
                        }
                        break;
                    case ServiceLifetime.Transient:
                        if (item.GetInterfaces().Length == 1)
                        {
                            services.AddSingleton(item);
                        }
                        else
                        {
                            foreach (var interfaceitem in item.GetInterfaces())
                            {
                                services.AddTransient(interfaceitem, item);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return services;
        }
    }
}
