using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Dependency;
using System.Data;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Data.Schema;
using System.Linq;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Logger;

namespace Simple.dotNet.Core.Domain
{
    /// <summary>
    /// 服务基类
    /// </summary>
    public abstract class AppServiceBase
    {
        protected IWriteRepository WriteRepository { get; }
        protected IReadRepository ReadRepository { get; }
        protected ILogger Logger { get; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// 数据库类型
        /// </summary>
        private readonly DatabaseType _type;

        /// <summary>
        /// 构造连接对象，默认（sql server数据库）
        /// </summary>
        /// <param name="connectionString"></param>
        public AppServiceBase(string connectionString) : this(connectionString, DatabaseType.SqlServer)
        {

        }
        public AppServiceBase(string connectionString, DatabaseType type) : this()
        {
            this._connectionString = connectionString;
            this._type = type;
        }
        public AppServiceBase()
        {
            this.Logger = IocCollection.Resolve<ILogger>() ?? new DefaultLogger();
            this.WriteRepository = IocCollection.Resolve<IWriteRepository>();
            this.ReadRepository = IocCollection.Resolve<IReadRepository>();
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected IDapperDatabase CreateDatabase(IsolationLevel level = IsolationLevel.Unspecified) => CreateDatabase(_connectionString, level, _type);
        protected IDapperDatabase CreateDatabase(string connectionString, IsolationLevel level, DatabaseType type)
        {
            return DbConnectionFactory.CreateDatabase(connectionString, level, type);
        }
        /// <summary>
        /// 公共排序功能
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sorts"></param>
        /// <returns></returns>
        protected bool SaveSort<TEntity>(int[] sorts)
        {
            using (IDapperDatabase db = CreateDatabase(IsolationLevel.ReadUncommitted))
            {
                short sort = (short)sorts.Length;
                string tableName = typeof(TEntity).GetTableName();
                ColumnProperty column = typeof(TEntity).GetTableKey().FirstOrDefault();
                foreach (var item in sorts)
                {
                    sort--;
                    db.Execute(CommandType.Text, $"UPDATE {tableName} SET Sort={sort} WHERE {column.Name}={item}", null);
                }
                db.Commit();
            }
            return true;
        }
        /// <summary>
        /// 公共排序功能（需指定database对象）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="db"></param>
        /// <param name="sorts"></param>
        protected void SaveSort<TEntity>(IDapperDatabase db, int[] sorts)
        {
            short sort = (short)sorts.Length;
            string tableName = typeof(TEntity).GetTableName();
            ColumnProperty column = typeof(TEntity).GetTableKey().FirstOrDefault();
            foreach (var item in sorts)
            {
                sort--;
                db.Execute(CommandType.Text, $"UPDATE {tableName} SET Sort={sort} WHERE {column.Name}={item}", null);
            }
        }
    }
}
