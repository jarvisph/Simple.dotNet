using System;

namespace Simple.dotNet.Core.Domain.Dto.Page
{
    /// <summary>
    /// 搜索接口
    /// </summary>
    public interface ISearchResultRequest
    {
        /// <summary>
        /// 搜索,全局
        /// </summary>
        string Search { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime? EndTime { get; set; }
    }
}
