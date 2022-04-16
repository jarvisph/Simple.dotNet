using Microsoft.AspNetCore.Builder;
using Simple.Core.Http;
using Simple.Web.Jwt;
using Simple.Web.Middleware;

namespace Simple.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSimple(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpContext();
            app.UseMiddleware<JWTMiddleware>();
            return app;
        }
    }
}
