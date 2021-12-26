namespace Simple.dotNet.Core.Domain.Dto.Page
{
    public interface IPagedResultRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        int Page { get; set; }
        /// <summary>
        /// 每页显示总数
        /// </summary>
        int Limit { get; set; }
    }
}
