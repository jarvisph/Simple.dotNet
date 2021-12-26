using Microsoft.AspNetCore.Builder;
using Simple.dotNet.Core.Http;
using Simple.dotNet.Web.Middleware;

namespace Simple.dotNet.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSimple(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpContext();
            //app.UseRouting();
            app.UseMiddleware<AuthorizationMiddleware>();
            //app.UseEndpoints(opt =>
            //{
            //    opt.MapControllers();
            //});

            return app;
        }
    }
}
