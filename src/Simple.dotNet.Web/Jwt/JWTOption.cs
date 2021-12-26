using Microsoft.IdentityModel.Tokens;
using System;

namespace Simple.dotNet.Web.Jwt
{
    public struct JWTOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audience">订阅者</param>
        /// <param name="issuer">发起人</param>
        /// <param name="expire">过期时间 单位 小时</param>
        public JWTOption(string audience, string issuer, int expire)
        {
            this.Audience = audience;
            this.Issuer = issuer;
            this.Expire = expire;
            this.Credentials = null;
        }
        public JWTOption(int time)
        {
            this.Audience = "net core";
            this.Issuer = "net core";
            this.Expire = time;
            this.Credentials = null;
        }
        /// <summary>
        ///  订阅者
        /// </summary>
        public string Audience;
        /// <summary>
        /// 签名证书
        /// </summary>
        public SigningCredentials Credentials;
        /// <summary>
        /// 发起人
        /// </summary>
        public string Issuer;
        /// <summary>
        /// 过期时间 单位（小时）
        /// </summary>
        public int Expire;
    }
}
