using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Data
{
    /// <summary>
    /// db连接对象
    /// </summary>
    public class DbConnection
    {
        public DbConnection(string connectionString, DatabaseType type)
        {
            this.ConnectionString = connectionString;
            this.Type = type;
        }
        public string ConnectionString { get; set; }
        public DatabaseType Type { get; set; }
    }
}
