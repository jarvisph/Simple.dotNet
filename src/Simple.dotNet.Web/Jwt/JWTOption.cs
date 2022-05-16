using Simple.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple.Web.Jwt
{
    public class JWTOption : QuerySetting
    {
        public JWTOption(string queryString) : base(queryString)
        {

        }
        /// <summary>
        ///  订阅者
        /// </summary>
        [Column("audience")]
        public string Audience { get; set; } = string.Empty;
        /// <summary>
        /// token名称
        /// </summary>
        [Column("tokenname")]
        public string TokenName { get; set; } = string.Empty;
        /// <summary>
        /// 发起人
        /// </summary>
        [Column("issuer")]
        public string Issuer { get; set; } = string.Empty;
        /// <summary>
        /// 签名密钥
        /// </summary>
        [Column("secret")]
        public string Secret { get; set; } = string.Empty;
        /// <summary>
        /// 过期时间
        /// </summary>
        [Column("expire")]
        public int? Expire { get; set; }
    }
}
