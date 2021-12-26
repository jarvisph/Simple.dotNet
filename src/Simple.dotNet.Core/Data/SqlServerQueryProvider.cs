using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Dapper.Expressions;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Core.Data
{
    internal class SqlServerQueryProvider : IQueryProvider
    {
        /// <summary>
        /// 数据库执行对象
        /// </summary>
        protected readonly IDatabase _database;
        public SqlServerQueryProvider(IDatabase database)
        {
            _database = database;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(DapperQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException exp)
            {
                throw exp.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DapperQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return default;
        }
        public IEnumerable<TResult> Executeable<TResult>(Expression expression)
        {
            using (IExpressionVisitor visitor = new SqlServerExpressionVisitor(expression))
            {
                string sql = visitor.GetSelect(out DynamicParameters parameters, out Type type);
                IDataReader reader = _database.ExecuteReader(CommandType.Text, sql, parameters);
                while (reader.Read())
                {
                    object source = Activator.CreateInstance(type);
                    yield return (TResult)reader.GetReaderData(source);
                }
                reader.Close();
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            TResult result = default;
            using (IExpressionVisitor visitor = new SqlServerExpressionVisitor(expression))
            {
                string sql = visitor.GetSelect(out DynamicParameters parameters, out MethodType type);
                switch (type)
                {
                    case MethodType.None:
                        break;
                    case MethodType.Any:
                        result = (TResult)(object)(_database.ExecuteScalar(CommandType.Text, sql, parameters) != null);
                        break;
                    case MethodType.Count:
                        object value = _database.ExecuteScalar(CommandType.Text, sql, parameters);
                        result = (TResult)(object)(value == null ? 0 : (int)value);
                        break;
                    case MethodType.FirstOrDefault:
                        IDataReader reader = _database.ExecuteReader(CommandType.Text, sql, parameters);
                        while (reader.Read())
                        {
                            result = reader.GetReaderData<TResult>();
                        }
                        reader.Close();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
