using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Core.Dapper;
using Simple.Core.Data;
using Simple.Core.Data.Repository;
using Simple.Core.Dependency;
using Simple.Core.Localization;
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
