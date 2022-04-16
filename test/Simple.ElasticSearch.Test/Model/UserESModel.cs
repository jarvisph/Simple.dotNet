using Nest;
using Simple.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Elasticsearch.Test.Model
{
    /// <summary>
    /// 会员表
    /// </summary>
    [ElasticsearchType(IdProperty = "ID"), ElasticSearchIndex("user", AliasNames = new[] { "user" })]
    public class UserESModel : IDocument
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public int SiteID { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        public bool IsTest { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public Guid LoginIP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
