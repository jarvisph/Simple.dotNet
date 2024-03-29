﻿using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;

namespace Simple.Core.Data
{
    /// <summary>
    /// SqlServer
    /// </summary>
    public class SqlServerConnectionProvider : ISqlServerConnectionProvider
    {
        public IDapperDatabase GetDatabase(string connectionString, IsolationLevel level)
        {
            return new SqlServerRepository(connectionString, level);
        }

        public IDbConnection GetDbConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new NullReferenceException("connectionstring is null");
            }
            return new SqlConnection(connectionString);
        }
    }
}
