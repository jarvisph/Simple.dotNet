using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Core.Extensions;
using Simple.Core.Helper;

namespace Simple.Core.Domain.Dto
{
    [Serializable]
    public struct PagedResult<T>
    {
        public PagedResult(List<T> items, long total)
        {
            this.Items = items;
            this.Total = total;
            this.Extend = null;
        }
        public PagedResult(List<T> items, long total, object extend)
        {
            this.Items = items;
            this.Total = total;
            this.Extend = extend;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Items;
        /// <summary>
        /// 总记录数
        /// </summary>
        public long Total;
        /// <summary>
        /// 扩展数据
        /// </summary>
        public object Extend;
        public override string ToString()
        {
            return "{\"success\":1,\"message\":\""
                + StopwatchHelper.Milliseconds + " ms\",\"info\":{\"items\":"
                + this.Items.ToJson(JsonSettings.JsonSerializerSettings()) + ",\"total\":"
                + this.Total + ",\"extend\":"
                + this.Extend.ToJson(JsonSettings.JsonSerializerSettings()) + "}}";
        }
    }
}
