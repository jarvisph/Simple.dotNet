using System.Net;

namespace Simple.Core.Logger
{
    public class RequestLoggerModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 请求路径
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 返回包大小
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 请求内容
        /// </summary>
        public string RequestBody { get; set; }
        /// <summary>
        /// 返回内容
        /// </summary>
        public string ResponseBody { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 请求开始时间
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 请求结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// HttpStatus
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 平台代码
        /// </summary>
        public string Code { get; set; }
    }
}
