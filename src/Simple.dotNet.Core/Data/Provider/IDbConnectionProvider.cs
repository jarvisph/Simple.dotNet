using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Simple.Core.Dapper;

namespace Simple.Core.Data.Provider
{
    /// <summary>
    /// db 连接对象基类
    /// </summary>
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection(string connectionString);
        IDapperDatabase GetDatabase(string connectionString, IsolationLevel level);
    }
}
