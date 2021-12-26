using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Net.Dapper;
using Simple.Net.Data.Repository;
using Simple.Net.Dependency;
using Simple.Net.Localization;
using Simple.MySql.Test.Model;

namespace Simple.MySql.Test
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
