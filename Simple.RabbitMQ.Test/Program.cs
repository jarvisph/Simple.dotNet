using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Net.Dependency;
using Simple.Net.Localization;
using Simple.RabbitMQ.Test.Queues;
using System.Linq;
using System;

namespace Simple.RabbitMQ.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddRabbitProduce();
            services.AddDepency();
            services.AddRabbitConsumer();
            IRabbitProducer producer = IocCollection.Resolve<IRabbitProducer>();
            producer.Send(new TestQueue() { UserID = 1, UserName = "ÕÅÈý" }, ExchangeName.Test_Exchange);
            Console.Read();

        }
    }
}
