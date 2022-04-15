using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Simple.dotNet.Core.Authorization;
using System.Text;

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
                Expires = DateTime.Now.Add(options.Expire)
            });
            //生成令牌字符串
            return handler.WriteToken(securityToken);
        }
    }
}
