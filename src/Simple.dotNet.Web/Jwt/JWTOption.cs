using Microsoft.IdentityModel.Tokens;
using System;

namespace Simple.dotNet.Web.Jwt
{
    public class JWTOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audience">订阅者</param>
        /// <param name="issuer">发起人</param>
        /// <param name="expire">过期时间 单位 小时</param>
        public JWTOption(string audience, string issuer, string Secret, TimeSpan expire)
        {
            this.Audience = audience;
            this.Issuer = issuer;
            this.Secret = Secret;
            this.Expire = expire;
            this.TokenName = "Token";
        }
        /// <summary>
        ///  订阅者
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// token名称
        /// </summary>
        public string TokenName { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 签名密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expire;
    }
}
