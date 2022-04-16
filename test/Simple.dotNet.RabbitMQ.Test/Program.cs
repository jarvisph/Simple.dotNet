using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Core.Dependency;
using Simple.Core.Localization;
using Simple.RabbitMQ.Test.Queues;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Simple.RabbitMQ.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDepency();
            services.AddRabbitMQ(new RabbitOption(AppsettingConfig.GetConnectionString("RabbitConnection")));
            for (int i = 0; i < 1000; i++)
            {
                new TestQueue() { UserID = 10000, UserName = "²âÊÔ" + i }.Send();
            }
            Thread.Sleep(-1);
        }
    }
}
