using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 消费标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        /// 默认构造，交换机与队列名称一致
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
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public int Unacked { get; set; }

    }
}
