using Simple.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Tool.Obs
{
    public class ObsSetting : QuerySetting
    {
        public ObsSetting(string queryString) : base(queryString)
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
