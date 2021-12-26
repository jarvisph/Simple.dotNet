using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Simple.dotNet.Core.Authorization;

namespace Simple.dotNet.Web.Jwt
{
    /// <summary>
    /// 授权Token
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static string CreateToken(JWTOption jwt, IEnumerable<Claim> claims)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(claims);
            //Jwt安全令牌
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                SigningCredentials = jwt.Credentials,
                Subject = identity,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.Add(TimeSpan.FromHours(jwt.Expire))
            });
            //生成令牌字符串
            return handler.WriteToken(securityToken);
        }
        public static string CreateToken(JWTOption jwt, int userId)
        {
            return CreateToken(jwt, new[] { new Claim("UserId", userId.ToString()) });
        }
        /// <summary>
        /// 获取ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetClaimsPrincipal(string token, JWTOption options)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) throw new AuthorizationException("非法授权");
            JwtSecurityToken jwt = handler.ReadJwtToken(token);
            if (jwt.Issuer != options.Issuer)
            {
                throw new AuthorizationException("非法Token");
            }
            if (DateTime.UtcNow > jwt.ValidTo)
            {
                throw new AuthorizationException("Token已过期");
            }
            if (jwt.Audiences.Any(c => c != options.Audience))
            {
                throw new AuthorizationException("非法订阅");
            }
            return new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims));
        }
        public static ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return new ClaimsPrincipal();
            JwtSecurityToken jwt = handler.ReadJwtToken(token);
            return new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims));
        }
    }
}
