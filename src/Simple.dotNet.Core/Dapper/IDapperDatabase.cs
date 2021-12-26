using System;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Repository;

namespace Simple.dotNet.Core.Dapper
{
    /// <summary>
    /// 原生及仓储方法
    /// </summary>
    public interface IDapperDatabase : IDatabase, IReadRepository, IWriteRepository, IProcedureRepository, IDisposable
    {

    }
}
