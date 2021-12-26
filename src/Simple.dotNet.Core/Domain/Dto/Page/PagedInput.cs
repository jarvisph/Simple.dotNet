namespace Simple.dotNet.Core.Domain.Dto.Page
{
    /// <summary>
    /// 分页参数显示总记录数
    /// </summary>
    public class PagedInput : IPagedResultRequest
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PagedInput()
        {
            Limit = 10;
            Page = 1;
        }
        /// <summary>
        /// 当前页数（默认第一页）
        /// </summary>
        public virtual int Page { get; set; }
        /// <summary>
        /// 最大显示记录数（默认10条）
        /// </summary>
        public virtual int Limit { get; set; }
    }
}
