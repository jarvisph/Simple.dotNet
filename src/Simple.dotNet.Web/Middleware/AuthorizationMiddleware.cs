using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;
using Simple.dotNet.Web.Jwt;

namespace Simple.dotNet.Web.Middleware
{
    /// <summary>
    /// 登陆授权中间件
    /// </summary>
    public class AuthorizationMiddleware
    {
        /// <summary>
        /// 代理管道
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="next"></param>
        public AuthorizationMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public virtual Task Invoke(HttpContext context)
        {
            StringValues value = context.Request.Headers.Where(c => c.Key == "Token").FirstOrDefault().Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                value = context.Request.Headers.Where(c => c.Key == "Authorization").FirstOrDefault().Value;
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                context.User = JWTHelper.GetClaimsPrincipal(value);
            }
            return _next(context);
        }
    }
}
