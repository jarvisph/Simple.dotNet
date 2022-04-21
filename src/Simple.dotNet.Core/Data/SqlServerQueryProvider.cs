using Dapper;
using Simple.Core.Dapper;
using Simple.Core.Data.Expressions;
using Simple.Core.Data.Schema;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Core.Data
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
            using (ISqlExpressionVisitor visitor = new SqlServerExpressionVisitor(expression))
            {
                string sql = visitor.GetSqlText(out DynamicParameters parameters);
                Console.WriteLine($"SQL语句：{sql}");
                Console.WriteLine($"Cell：{string.Join(",", visitor.Cells)}");
                Console.WriteLine($"条件值：{string.Join(",", parameters.ParameterNames.Select(c => $"{c}={parameters.Get<object>(c).GetString()}"))}");
                IDataReader reader = _database.ExecuteReader(CommandType.Text, sql, parameters);
                while (reader.Read())
                {
                    object source = Activator.CreateInstance(typeof(TResult));
                    yield return (TResult)reader.GetReaderData(source);
                }
                reader.Close();
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            TResult result = default;
            using (ISqlExpressionVisitor visitor = new SqlServerExpressionVisitor(expression))
            {
                string sql = visitor.GetSqlText(out DynamicParameters parameters);
                Console.WriteLine($"SQL语句：{sql}");
                Console.WriteLine($"Cell：{string.Join(",", visitor.Cells)}");
                Console.WriteLine($"条件值：{string.Join(",", parameters.ParameterNames.Select(c => $"{c}={parameters.Get<object>(c).GetString()}"))}");
                if (visitor.Cells.Any(c => c == "Any"))
                {
                    result = (TResult)(object)(_database.ExecuteScalar(CommandType.Text, sql, parameters) != null);
                }
                else if (visitor.Cells.Any(c => c == "Count"))
                {
                    object value = _database.ExecuteScalar(CommandType.Text, sql, parameters);
                    result = (TResult)(object)(value == null ? 0 : (int)value);
                }
                else if (visitor.Cells.Any(c => c == "FirstOrDefault"))
                {
                    IDataReader reader = _database.ExecuteReader(CommandType.Text, sql, parameters);
                    while (reader.Read())
                    {
                        result = reader.GetReaderData<TResult>();
                    }
                    reader.Close();
                }

            }
            return result;
        }
    }
}
