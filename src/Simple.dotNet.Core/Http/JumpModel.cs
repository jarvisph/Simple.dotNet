using Simple.Core.Domain.Enums;
using Simple.Core.Domain.Model;
using System.Collections.Generic;

namespace Simple.Core.Http
{
    public class JumpModel
    {
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 代理
        /// </summary>
        public ProxySetting Proxy { get; set; }
        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public ContentType ContentType { get; set; }
        /// <summary>
        /// 请求内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 请求路径
        /// </summary>
        public string Url { get; set; }
    }
}
