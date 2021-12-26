using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Dependency;
using System.Data;
using Simple.dotNet.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Simple.dotNet.Core.Domain.Dto;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Data.Schema;
using System.Linq;
using Simple.dotNet.Core.Data;

namespace Simple.dotNet.Core.Domain
{
    /// <summary>
    /// 服务基类
    /// </summary>
    public abstract class AppServiceBase
    {
        /// <summary>
        /// 读写仓储
        /// </summary>
        protected IWriteRepository WriteRepository
        {
            get
            {
                return IocCollection.Resolve<IWriteRepository>();
            }
        }
        /// <summary>
        /// 只读仓储
        /// </summary>
        protected IReadRepository ReadRepository
        {
            get
            {
                return IocCollection.Resolve<IReadRepository>();
            }
        }
        /// <summary>
        /// http上下文（为web程序为null）
        /// </summary>
        protected HttpContext HttpContext
        {
            get
            {
                return Http.HttpContextAccessor.HttpContext;
            }
        }
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
        public AppServiceBase(string connectionString, DatabaseType type)
        {
            this._connectionString = connectionString;
            this._type = type;
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
        /// 添加错误消息（http上下文传递，非web可不用）
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ErrorMessageResult ErrorMessageResult(string message)
        {
            if (HttpContext == null) return default;
            ErrorMessageResult result = IocCollection.Resolve<ErrorMessageResult>();
            if (!message.IsNullOrWhiteSpace()) result.Add(message);
            return result;
        }
        /// <summary>
        /// 错误消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool ErrorMessage(string message)
        {
            this.ErrorMessageResult(message);
            return false;
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
