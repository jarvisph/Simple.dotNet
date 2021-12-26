using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Localization;
using Simple.dotNet.Sqlite.Test.Model;

namespace Simple.dotNet.Sqlite.Test
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
            services.AddSqlite(connectionString);
            IWriteRepository WriteDB = IocCollection.Resolve<IWriteRepository>();
            bool success = WriteDB.Delete<Connection>(c => c.ConnectionID == 1);
            using (IDapperDatabase db = DbConnectionFactory.CreateDatabase(connectionString, System.Data.IsolationLevel.Unspecified, DatabaseType.Sqlite))
            {
                success = db.Delete<Connection>(c => c.ConnectionID == 1);
            }
        }
    }
}
