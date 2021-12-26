using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Hub
{
    public static class IMApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIM(this IApplicationBuilder app)
        {
            app.UseMiddleware<IMMiddleware>();
            return app;
        }
    }
}
