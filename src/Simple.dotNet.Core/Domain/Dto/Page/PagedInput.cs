namespace Simple.Core.Domain.Dto.Page
{
    /// <summary>
    /// 分页参数显示总记录数
    /// </summary>
    public class PagedInput : IPagedResultRequest
    {
        /// <summary>
        /// 当前页数（默认第一页）
        /// </summary>
        public virtual int Page { get; set; } = 1;
        /// <summary>
        /// 最大显示记录数（默认10条）
        /// </summary>
        public virtual int Limit { get; set; } = 10;

        /// <summary>
        /// 排序类型
        /// </summary>
        public virtual string Order { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public virtual string Field { get; set; }
    }
}
