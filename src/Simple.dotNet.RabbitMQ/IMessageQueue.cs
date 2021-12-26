using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 所有消息体必须集成此接口
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// 错误次数，作用于重试机制
        /// </summary>
        public int ErrorCount { get; set; }
    }
}
