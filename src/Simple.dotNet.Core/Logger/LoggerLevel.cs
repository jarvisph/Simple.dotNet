using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Logger
{
    public enum LoggerLevel : byte
    {
        /// <summary>
        /// 跟踪
        /// </summary>
        Trace,
        /// <summary>
        /// bug
        /// </summary>
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }
}
