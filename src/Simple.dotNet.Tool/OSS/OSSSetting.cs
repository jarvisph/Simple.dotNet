using Simple.Core.Domain;
using System.ComponentModel;

namespace Simple.Tool.OSS
{
    /// <summary>
    /// OSS 的参数设定
    /// </summary>
    public class OSSSetting : QuerySetting
    {
        public OSSSetting(string queryString) : base(queryString)
        {
        }

        /// <summary>
        /// EndPoint（地域节点）
        /// </summary>
        [Description("EndPoint")]
        public string? endpoint { get; set; }

        /// <summary>
        /// Authorized account（RAM管理内）
        /// </summary>
        [Description("Authorized account")]
        public string? accessKeyId { get; set; }

        /// <summary>
        /// Authorization key（RAM管理内）
        /// </summary>
        [Description("Authorization key")]
        public string? accessKeySecret { get; set; }

        /// <summary>
        /// 存储对象名字（backet的名字）
        /// </summary>
        [Description("backetName")]
        public string? bucketName { get; set; }
    }
}
