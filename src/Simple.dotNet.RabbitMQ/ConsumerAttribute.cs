using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// 消费标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        /// 默认构造，交换器与队列名称一致
        /// </summary>
        /// <param name="exchangename">交换机名称</param>
        public ConsumerAttribute(string exchangename)
        {
            this.ExchangeName = exchangename;
            this.QueueName = exchangename;
            this.Type = ExchangeType.Fanout;
            this.Unacked = 8;
        }
        public string Type { get; set; }
        /// <summary>
        /// 交换器
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 队列
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public string? RoutingKey { get; set; }
        /// <summary>
        /// 预取消息数量
        /// </summary>
        public ushort Unacked { get; set; }

    }
}
