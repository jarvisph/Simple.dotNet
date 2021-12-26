using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Dapper.Expressions;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Schema;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.MySql
{
    public class MySqlRepository : DapperRepositoryBase, IDapperDatabase
    {
        public MySqlRepository(string connectionString) : base(connectionString, IsolationLevel.Unspecified, DatabaseType.MySql)
        {

        }
        public MySqlRepository(string connectionString, IsolationLevel level) : base(connectionString, level, DatabaseType.MySql)
        {

        }
        public override bool Any<TEntity>()
        {
            string sql = $"SELECT 0 WHERE EXISTS(SELECT 0 FROM {typeof(TEntity).GetTableName()});";
            return this.ExecuteScalar(CommandType.Text, sql, null) != null;
        }

        public override bool Any<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"SELECT 0 WHERE EXISTS(SELECT 0 FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)});";
                return this.ExecuteScalar(CommandType.Text, sql, parameters) != null;
            }
        }

        public override int Count<TEntity>()
        {
            string sql = $"SELECT COUNT(0) FROM {typeof(TEntity).GetTableName()};";
            object value = this.ExecuteScalar(CommandType.Text, sql, null);
            return value == null ? 0 : (int)((long)value);
        }

        public override int Count<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"SELECT COUNT(0) FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)};";
                object value = this.ExecuteScalar(CommandType.Text, sql, parameters);
                return value == null ? 0 : (int)((long)value);
            }
        }

        public override bool Delete<TEntity>(TEntity entity)
        {
            DynamicParameters parameters = new DynamicParameters();
            Stack<string> fields = new Stack<string>();
            foreach (ColumnProperty column in typeof(TEntity).GetColumns().Where(c => c.IsKey))
            {
                fields.Push($"{column.Name}=@{column.Name}");
                parameters.Add(column.Name, column.Property.GetValue(entity));
            }
            string sql = $"DELETE FROM {typeof(TEntity).GetTableName()} WHERE {string.Join(" AND ", fields.ToArray())};";
            return this.Execute(CommandType.Text, sql, parameters) > 0;
        }

        public override bool Delete<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"DELETE FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)};";
                return this.Execute(CommandType.Text, sql, parameters) > 0;
            }
        }


        public override TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                IEnumerable<ColumnProperty> columns = typeof(TEntity).GetColumns();
                string sql = $"SELECT  {string.Join(",", columns.Select(c => c.Name).ToArray())} FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)} LIMIT 0,1;";
                IDataReader reader = this.ExecuteReader(CommandType.Text, sql, parameters);
                TEntity entity = default;
                while (reader.Read())
                {
                    entity = reader.GetReaderData<TEntity>();
                }
                reader.Close();
                return entity;
            }
        }

        public override TValue FirstOrDefault<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"SELECT {field.GetFieldName()} FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)} LIMIT 0,1;";
                object value = this.ExecuteScalar(CommandType.Text, sql, parameters);
                if (value == null) return default;
                return (TValue)value;
            }
        }


        public override IEnumerable<TEntity> GetAll<TEntity>()
        {
            IEnumerable<ColumnProperty> columns = typeof(TEntity).GetColumns();
            string sql = $"SELECT {string.Join(",", columns.Select(c => c.Name).ToArray())} FROM {typeof(TEntity).GetTableName()};";
            IDataReader reader = this.ExecuteReader(CommandType.Text, sql, null);
            while (reader.Read())
            {
                yield return reader.GetReaderData<TEntity>();
            }
            reader.Close();
        }
        public override IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                IEnumerable<ColumnProperty> columns = typeof(TEntity).GetColumns();
                string sql = $"SELECT {string.Join(",", columns.Select(c => c.Name).ToArray())} FROM {typeof(TEntity).GetTableName()} WHERE {exp.GetCondition(out DynamicParameters parameters)};";
                IDataReader reader = this.ExecuteReader(CommandType.Text, sql, parameters);
                while (reader.Read())
                {
                    yield return reader.GetReaderData<TEntity>();
                }
                reader.Close();
            }
        }

        public override bool Insert<TEntity>(TEntity entity)
        {
            IEnumerable<ColumnProperty> fields = typeof(TEntity).GetColumns().Where(c => !c.Identity && !c.IsNull);
            string sql = $"INSERT INTO {typeof(TEntity).GetTableName()} ({string.Join(",", fields.Select(c => c.Name).ToArray())}) VALUES ({string.Join(",", fields.Select(c => $"@{c.Name}"))})";
            DynamicParameters parameters = new DynamicParameters();
            foreach (ColumnProperty property in fields)
            {
                parameters.Add(property.Name, property.Property.GetValue(entity));
            }
            return this.Execute(CommandType.Text, sql, parameters) > 0;
        }

        public override bool InsertIdentity<TEntity>(TEntity entity)
        {
            IEnumerable<ColumnProperty> columns = typeof(TEntity).GetColumns();
            if (!columns.Any(c => c.Identity)) throw new InvalidOperationException("no identity");
            ColumnProperty identity = columns.Where(c => c.Identity).FirstOrDefault();
            IEnumerable<ColumnProperty> fields = columns.Where(c => !c.Identity);
            string sql = $"INSERT INTO {typeof(TEntity).GetTableName()} ({string.Join(",", fields.Select(c => c.Name).ToArray())}) VALUES ({string.Join(",", fields.Select(c => $"@{c.Name}"))});SELECT LAST_INSERT_ID();";
            DynamicParameters parameters = new DynamicParameters();
            foreach (ColumnProperty property in fields)
            {
                parameters.Add(property.Name, property.Property.GetValue(entity));
            }
            object value = this.ExecuteScalar(CommandType.Text, sql, parameters);
            if (value == null || value == DBNull.Value) return false;
            identity.Property.SetValue(entity, Convert.ChangeType(value, identity.Property.PropertyType));
            return true;
        }

        public override bool Update<TEntity, TValue>(TEntity entity, Expression<Func<TEntity, TValue>> field, TValue value)
        {
            DynamicParameters parameters = new DynamicParameters();
            Stack<string> fields = new Stack<string>();
            foreach (ColumnProperty property in typeof(TEntity).GetTableKey())
            {
                fields.Push($"{property.Name}=@{property.Name}");
                parameters.Add(property.Name, property.Property.GetValue(entity));
            }
            parameters.Add("Value_01", value);
            string sql = $"UPDATE {typeof(TEntity).GetTableName()} SET {field.GetFieldName()}=@Value_01 WHERE {string.Join(" AND ", fields.ToArray())};";
            return this.Execute(CommandType.Text, sql, parameters) > 0;
        }

        public override bool Update<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"UPDATE {typeof(TEntity).GetTableName()} SET {field.GetFieldName()}=@Value_01 WHERE {exp.GetCondition(out DynamicParameters parameters)};";
                parameters.Add("Value_01", value);
                return this.Execute(CommandType.Text, sql, parameters) > 0;
            }
        }

        public override bool Plus<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string sql = $"UPDATE {typeof(TEntity).GetTableName()} SET {field.GetFieldName()}+=@Value_01 WHERE {exp.GetCondition(out DynamicParameters parameters)};";
                parameters.Add("Value_01", value);
                return this.Execute(CommandType.Text, sql, parameters) > 0;
            }
        }
        public override bool Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] fields)
        {
            using (IExpressionVisitor exp = this.GetExpressionVisitor(expression, DatabaseType.MySql))
            {
                string where = exp.GetCondition(out DynamicParameters parameters);
                Stack<string> update_fields = new Stack<string>();
                if (fields.Length == 0)
                {
                    foreach (ColumnProperty column in typeof(TEntity).GetColumns().Where(c => !c.Identity && !c.IsKey))
                    {
                        update_fields.Push($"{column.Name}=@{column.Name}_01");
                        parameters.Add($"{column.Name}_01", column.Property.GetValue(entity));
                    }
                }
                else
                {
                    foreach (ColumnProperty column in typeof(TEntity).GetColumns(fields))
                    {
                        update_fields.Push($"{column.Name}=@{column.Name}_01");
                        parameters.Add($"{column.Name}_01", column.Property.GetValue(entity));
                    }
                }
                string sql = $"UPDATE {typeof(TEntity).GetTableName()} SET {string.Join(",", update_fields.ToArray())} WHERE {where};";
                return this.Execute(CommandType.Text, sql, parameters) > 0;
            }
        }

        public override DataSet GetDataSet(CommandType type, string cmdText, object param)
        {

            MySqlCommand command = (MySqlCommand)_connection.CreateCommand();
            command.CommandType = type;
            command.CommandText = cmdText;
            if (param != null)
            {
                foreach (PropertyInfo property in param.GetType().GetProperties())
                {
                    command.Parameters.Add(new MySqlParameter(property.Name, property.GetValue(param)));
                }
            }

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            if (_transaction != null) command.Transaction = (MySqlTransaction)_transaction;
            try
            {
                adapter.SelectCommand = command;
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new DapperException(cmdText, ex.Message, param);
            }
            finally
            {

            }
        }

        public override int Execute<TProcedure>(TProcedure procedure)
        {
            this.GetProcedureParameters(procedure, out DynamicParameters parameters);
            return this.Execute(CommandType.StoredProcedure, typeof(TProcedure).GetTableName(), parameters);
        }

        public override T ExecuteScalar<T, TProcedure>(TProcedure procedure)
        {
            this.GetProcedureParameters(procedure, out DynamicParameters parameters);
            return (T)this.ExecuteScalar(CommandType.StoredProcedure, typeof(TProcedure).GetTableName(), parameters);
        }
        public override IDataReader GetDataReader<TProcedure>(TProcedure procedure)
        {
            this.GetProcedureParameters(procedure, out DynamicParameters parameters);
            return this.ExecuteReader(CommandType.StoredProcedure, typeof(TProcedure).GetTableName(), parameters);
        }



        public override IQueryable<TEntity> Query<TEntity>()
        {
            throw new NotImplementedException();
        }
    }
}
