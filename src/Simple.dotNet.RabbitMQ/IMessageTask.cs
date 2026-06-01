using System;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 异步任务ID
    /// </summary>
    public interface IMessageTask
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        Guid TaskID { get; set; }
    }
}
