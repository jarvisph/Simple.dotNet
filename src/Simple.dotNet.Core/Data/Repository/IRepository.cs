using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Data.Repository
{
    public interface IRepository : IDatabase, IReadRepository, IWriteRepository, IProcedureRepository, IDisposable
    {

    }
}
