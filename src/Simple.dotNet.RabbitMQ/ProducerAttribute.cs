using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.RabbitMQ
{
    /// <summary>
    /// 生产实体标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProducerAttribute : Attribute
    {
        public ProducerAttribute(string name)
        {
            this.Name = name;
            this.Type = ExchangeType.Fanout;
        }
        public string Name { get; }
        /// <summary>
        /// fanout 将消息发送交换器绑定的所有队列
        /// direct 由路由绑定，指定队列，完全匹配
        /// topic 由路由绑定，指定队列，模糊匹配
        /// headers 发送内容，由headers属性进行匹配，性能差，不建议使用
        /// </summary>
        public string Type { get; set; }
    }

}
