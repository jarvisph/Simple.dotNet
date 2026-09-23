using Newtonsoft.Json;

namespace Simple.Core.Domain
{
    public class JsonResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        /// <summary>
        /// 提示消息
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        /// <summary>
        /// 数据信息
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; } = 200;
    }
}
