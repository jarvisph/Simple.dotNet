using Microsoft.IdentityModel.Tokens;
using Simple.Core.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simple.Web.Jwt
{
    /// <summary>
    /// 授权Token
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string CreateToken(JWTOption options, IEnumerable<Claim> claims)
        {
            var key = Encoding.ASCII.GetBytes(options.Secret);
            var handler = new JwtSecurityTokenHandler();
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            //Jwt安全令牌
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = options.Issuer,
                Audience = options.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = identity,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddHours(options.Expire ?? 1)
            });
            //生成令牌字符串
            return handler.WriteToken(securityToken);
        }

        public static JwtSecurityToken ParseJwt(string token)
        {

            var handler = new JwtSecurityTokenHandler();

            // 检查 Token 格式是否正确
            if (!handler.CanReadToken(token))
            {
                throw new AuthorizationException("无效的Token");
            }

            // 解析 Token
            return handler.ReadJwtToken(token);
        }
    }
}
