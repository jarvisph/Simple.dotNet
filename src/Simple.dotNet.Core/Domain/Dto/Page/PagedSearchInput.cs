using System;

namespace Simple.Core.Domain.Dto.Page
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class PagedSearchInput : PagedInput, IPagedResultRequest
    {
        /// <summary>
        /// 搜索
        /// </summary>
        public virtual string Search { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
