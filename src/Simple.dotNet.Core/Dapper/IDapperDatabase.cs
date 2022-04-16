using System;
using Simple.Core.Data;
using Simple.Core.Data.Repository;

namespace Simple.Core.Dapper
{
    /// <summary>
    /// 原生及仓储方法
    /// </summary>
    public interface IDapperDatabase : IDatabase, IReadRepository, IWriteRepository, IProcedureRepository, IDisposable
    {

    }
}
