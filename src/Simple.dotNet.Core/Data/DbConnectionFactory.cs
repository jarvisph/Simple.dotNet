using Microsoft.Data.SqlClient;
using System.Data;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;
using Simple.Core.Dependency;

namespace Simple.Core.Data
{
    /// <summary>
    /// 数据库连接工厂
    /// </summary>
    public class DbConnectionFactory
    {
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string connectionString, DatabaseType type = DatabaseType.SqlServer)
        {
            IDbConnectionProvider _provider = null;
            //获取配置进行转换
            switch (type)
            {
                case DatabaseType.SqlServer:
                    _provider = IocCollection.Resolve<ISqlServerConnectionProvider>();
                    break;
                case DatabaseType.MySql:
                    _provider = IocCollection.Resolve<IMySqlConnectionProvider>();
                    break;
                case DatabaseType.Sqlite:
                    _provider = IocCollection.Resolve<ISqliteConnectionProvider>();
                    break;
                case DatabaseType.Oracle:

                    break;
            }
            return _provider.GetDbConnection(connectionString);
        }
        public static IDapperDatabase CreateDatabase(string connectionString, IsolationLevel level, DatabaseType type)
        {
            IDbConnectionProvider _provider = null;
            //获取配置进行转换
            switch (type)
            {
                case DatabaseType.SqlServer:
                    _provider = IocCollection.Resolve<ISqlServerConnectionProvider>();
                    break;
                case DatabaseType.MySql:
                    _provider = IocCollection.Resolve<IMySqlConnectionProvider>();
                    break;
                case DatabaseType.Sqlite:
                    _provider = IocCollection.Resolve<ISqliteConnectionProvider>();
                    break;
                case DatabaseType.Oracle:

                    break;
            }
            return _provider.GetDatabase(connectionString, level);
        }
    }
}
