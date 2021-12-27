using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.dotNet.Core.Healthy;

namespace Simple.dotNet.Healthy.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void TestMethod1()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddHealthy(new HealthyOptions
            {
                Address = "http://127.0.0.1:3000",
                Host = "localhost",
                ServiceName = "Simple.dotNet.Healthy.Test",
            });
        }
    }
}
