using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.RabbitMQ.Test.Queues;

namespace Simple.RabbitMQ.Test.Consumer
{
    public class TestConsumer : RabbitConsumerBase<TestQueue>
    {
        public override string Queue { get; set; } = ExchangeName.Test_Exchange + ".Test";
        public override string Exchange { get; set; } = ExchangeName.Test_Exchange;
        public override string Type { get; set; } = ExchangeType.Direct;

        public override bool Invoke(TestQueue message, object sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));
            return true;
        }
    }
}
