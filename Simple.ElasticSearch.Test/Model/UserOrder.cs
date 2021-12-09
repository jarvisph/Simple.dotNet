using Simple.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ElasticSearch.Test.Model
{
    /// <summary>
    /// 会员订单
    /// </summary>
    [ElasticSearchIndex("user_order", AliasNames = new[] { "user_order" })]
    public class UserOrder : IDocument
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 会员ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 下单金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
