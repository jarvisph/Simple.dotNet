using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dependency;
using Simple.Core.Localization;
using Simple.RabbitMQ;
using Simple.RabbitMQ.Test.Queues;

IServiceCollection services = new ServiceCollection();
services.AddDepency();
RabbitOption options = new RabbitOption(AppsettingConfig.GetConnectionString("RabbitConnection"));
string connectionString = options.ToString();
services.AddRabbitMQ(options);
Task.Run(() =>
{
    while (true)
    {
        new TestQueue() { UserID = 10000, UserName = "测试01" }.Send();
    }
});
Thread.Sleep(-1);