using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Simple.dotNet.Core.Http
{
    public static class HttpContextAccessor
    {
        [ContextStatic]
        private static IApplicationBuilder _applicationBuilder;
        public static void UseHttpContext(this IApplicationBuilder app)
        {
            _applicationBuilder = app;
        }
        public static HttpContext HttpContext
        {
            get
            {
                // 未执行赋值方法（在非Web环境中）
                if (_applicationBuilder == null) return null;
                IHttpContextAccessor factory = _applicationBuilder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                return factory.HttpContext;
            }
        }
    }
}
