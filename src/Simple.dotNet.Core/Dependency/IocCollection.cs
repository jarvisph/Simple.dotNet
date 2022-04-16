using System;
using Simple.Core.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Simple.Core.Dependency
{
    /// <summary>
    /// IOC容器，统一管理
    /// </summary>
    public static class IocCollection
    {
        private static IServiceCollection _services;
        public static void AddCollection(this IServiceCollection services)
        {
            _services = services;
        }
        private static IServiceProvider _provider;
        private static IServiceProvider Provider
        {
            get
            {
                if (_services == null) return null;
                if (HttpContextAccessor.HttpContext != null)
                    _provider = HttpContextAccessor.HttpContext.RequestServices;
                if (_provider == null)
                    _provider = _services.BuildServiceProvider();
                return _provider;
            }
        }
        /// <summary>
        /// 获取容器对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Provider == null ? default : Provider.GetService<T>();
        }
        public static IServiceCollection AddSingleton(Type type)
        {
            return _services.AddSingleton(type);
        }
        public static IServiceCollection AddScoped(Type type)
        {
            return _services.AddScoped(type);
        }
    }
}
