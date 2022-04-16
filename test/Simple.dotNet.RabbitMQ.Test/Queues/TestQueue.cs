using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RabbitMQ.Test.Queues
{
    /// <summary>
    /// 消息体
    /// </summary>
    [Producer(ExchangeName.Test_Exchange)]
    public class TestQueue : IMessageQueue
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ErrorCount { get; set; }
    }
}
