using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Core.Dependency;
using Simple.Core.Domain.Enums;

namespace Simple.Redis.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDepency();
            services.AddRedis("");
            TestCacheService _testCacheService = IocCollection.Resolve<TestCacheService>();
            _testCacheService.SaveHash(new UserModel
            {
                Email = "zhangsan@qq.com",
                Id = 1,
                Name = "����",
                Password = "a123456",
                Status = UserStatus.Normal
            });
            UserModel user = _testCacheService.GetHash();
        }
    }
    public class TestCacheService : RedisDatabase, ISingletonDependency
    {
        protected override int Db => 1;
        public TestCacheService()
        {

        }
        public void SaveHash(UserModel user)
        {
            this.Redis.HashSet("USER", user);
        }
        public UserModel GetHash()
        {
            return this.Redis.HashGetAll("USER").GetReidsHashValue<UserModel>();
        }
    }
}
