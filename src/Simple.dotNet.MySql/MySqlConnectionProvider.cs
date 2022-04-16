using MySql.Data.MySqlClient;
using System;
using System.Data;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;

namespace Simple.MySql
{
    /// <summary>
    /// my sql
    /// </summary>
    public class MySqlConnectionProvider : IMySqlConnectionProvider
    {
        public IDapperDatabase GetDatabase(string connectionString, IsolationLevel level)
        {
            return new MySqlRepository(connectionString, level);
        }

        public IDbConnection GetDbConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new NullReferenceException("connectionstring is null");
            }
            return new MySqlConnection(connectionString);
        }
    }
}
