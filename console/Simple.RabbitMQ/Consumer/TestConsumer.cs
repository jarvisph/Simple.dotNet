using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Simple.RabbitMQ;
using Simple.RabbitMQ.Test.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RabbitMQ.Test.Consumer
{
    [Consumer("Test_Exchange")]
    public class TestConsumer : RabbitConsumerBase
    {
        public override void Invoke(string message, object sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine(message);
            Random random = new Random();
            Thread.Sleep(random.Next(10, 100));
        }
    }
    [Consumer("Test_Exchange")]
    public class TestModelConsumer : RabbitConsumerBase<TestQueue>
    {
        public override void Invoke(TestQueue message, object sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine(message.UserName);
            Random random = new Random();
            Thread.Sleep(random.Next(10, 100));
        }
    }
}
