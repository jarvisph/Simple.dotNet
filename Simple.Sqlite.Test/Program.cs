using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Net.Dapper;
using Simple.Net.Data;
using Simple.Net.Data.Repository;
using Simple.Net.Dependency;
using Simple.Net.Localization;
using Simple.Sqlite.Test.Model;

namespace Simple.Sqlite.Test
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
