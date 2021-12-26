using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Localization;
using Simple.dotNet.MySql.Test.Model;

namespace Simple.dotNet.MySql.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            string connectionString = AppsettingConfig.GetConnectionString("DbConnection");
            IServiceCollection services = new ServiceCollection();
            services.AddDepency();
            services.AddMySql(connectionString);
            IReadRepository ReadDB = IocCollection.Resolve<IReadRepository>();
            bool any = ReadDB.Any<User>();
            IWriteRepository WriteDB = IocCollection.Resolve<IWriteRepository>();
            any = WriteDB.Any<User>();
           
        }
    }
}
