using Simple.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Domain.Model
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class SettingModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 枚举
        /// </summary>
        public string Enum { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public FormType Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
