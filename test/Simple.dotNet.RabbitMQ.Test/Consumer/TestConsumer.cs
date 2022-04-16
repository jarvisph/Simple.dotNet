using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.dotNet.RabbitMQ.Test.Queues;

namespace Simple.dotNet.RabbitMQ.Test.Consumer
{
    [Consumer("Test_Exchange")]
    public class TestConsumer : RabbitConsumerBase
    {
        public override void Invoke(string message, object sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));
        }
    }
    public class TestModelConsumer : RabbitConsumerBase<TestQueue>
    {
        public override void Invoke(TestQueue message, object sender, BasicDeliverEventArgs args)
        {
            throw new NotImplementedException();
        }
    }


}
