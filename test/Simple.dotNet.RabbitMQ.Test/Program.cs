using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Localization;
using Simple.dotNet.RabbitMQ.Test.Queues;
using System.Linq;
using System;

namespace Simple.dotNet.RabbitMQ.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddRabbitProduce("");
            services.AddDepency();
            services.AddRabbitConsumer();
            new TestQueue() { UserID = 1, UserName = "ÕÅÈý" }.Send();
            Console.Read();

        }
    }
}
