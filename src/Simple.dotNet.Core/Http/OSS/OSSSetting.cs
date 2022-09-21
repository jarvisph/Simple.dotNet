using Simple.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Http.OSS
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
        /// 授权账户（RAM管理内）
        /// </summary>
        [Description("授权账户")]
        public string? accessKeyId { get; set; }

        /// <summary>
        /// 授权密钥（RAM管理内）
        /// </summary>
        [Description("授权密钥")]
        public string? accessKeySecret { get; set; }

        /// <summary>
        /// 存储对象名字（backet的名字）
        /// </summary>
        [Description("backetName")]
        public string? bucketName { get; set; }
    }
}
