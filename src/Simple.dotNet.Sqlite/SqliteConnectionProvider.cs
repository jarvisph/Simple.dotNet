using Microsoft.Data.Sqlite;
using System;
using System.Data;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;

namespace Simple.Sqlite
{
    /// <summary>
    /// Sqlite
    /// </summary>
    public class SqliteConnectionProvider : ISqliteConnectionProvider
    {
        public IDapperDatabase GetDatabase(string connectionString, IsolationLevel level)
        {
            return new SqliteRepository(connectionString, level);
        }

        public IDbConnection GetDbConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new NullReferenceException("connectionstring is null");
            }
            return new SqliteConnection(connectionString);
        }
    }
}
