using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Simple.Core.Authorization;
using Simple.Core.Dependency;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.Jwt
{
    /// <summary>
    /// JWT中间件
    /// </summary>
    public class JWTMiddleware
    {
        /// <summary>
        /// 代理管道
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly JWTOption _options;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="next"></param>
        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
            _options = IocCollection.Resolve<JWTOption>();

        }
        public virtual Task Invoke(HttpContext context)
        {
            if (_options == null) return _next(context);
            try
            {
                StringValues value = context.Request.Headers.Where(c => c.Key == _options.TokenName).FirstOrDefault().Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_options.Secret);
                    handler.ValidateToken(value, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                    var jwt = (JwtSecurityToken)validatedToken;

                    context.User = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims));
                }
                return _next(context);
            }
            catch
            {
                throw new AuthorizationException();
            }
        }
    }
}
