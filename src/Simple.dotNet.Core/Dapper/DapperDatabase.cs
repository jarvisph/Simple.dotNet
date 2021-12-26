using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Simple.dotNet.Core.Dapper.Expressions;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Schema;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.Core.Dapper
{
    /// <summary>
    /// 底层类，执行数据库
    /// </summary>
    public abstract class DapperDatabase : IDatabase
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        protected readonly IDbConnection _connection;

        /// <summary>
        /// 开启事务
        /// </summary>
        protected readonly IDbTransaction _transaction;

        /// <summary>
        /// 成功之后执行的方法
        /// </summary>
        protected List<Action> _rollbackActions;

        public DapperDatabase(string connectionString, IsolationLevel level = IsolationLevel.Unspecified, DatabaseType type = DatabaseType.SqlServer)
        {
            _connection = DbConnectionFactory.CreateConnection(connectionString, type);
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            if (level != IsolationLevel.Unspecified && _transaction == null)
            {
                _transaction = _connection.BeginTransaction(level);
            }
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollBack()
        {
            if (_transaction == null) throw new DapperException("未开启事务");
            _transaction.Rollback();
        }
        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            if (_transaction == null) throw new DapperException("未开启事务");
            _transaction.Commit();
            if (_rollbackActions != null)
            {
                foreach (Action action in _rollbackActions)
                {
                    action.Invoke();
                }
            }
        }
        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="action"></param>
        public void Collback(Action action)
        {
            if (_rollbackActions == null) _rollbackActions = new List<Action>();
            _rollbackActions.Add(action);
        }
        #region 执行数据库动作

        public int Execute(string cmdText) => this.Execute(cmdText, null);
        public int Execute(string cmdText, object param) => this.Execute(CommandType.StoredProcedure, cmdText, param);
        public int Execute(CommandType type, string cmdText, object param)
        {
            try
            {
                return _connection.Execute(cmdText, param, _transaction, null, type);
            }
            catch (Exception ex)
            {
                throw new DapperException(cmdText, ex.Message, param);
            }
            finally
            {

            }
        }

        public object ExecuteScalar(string cmdText) => this.ExecuteScalar(cmdText, null);
        public object ExecuteScalar(string cmdText, object param) => this.ExecuteScalar(CommandType.StoredProcedure, cmdText, param);
        public object ExecuteScalar(CommandType type, string cmdText, object param)
        {
            try
            {
                return _connection.ExecuteScalar(cmdText, param, _transaction, null, type);
            }
            catch (Exception ex)
            {
                throw new DapperException(cmdText, ex.Message, param);
            }
            finally
            {

            }
        }
        public IDataReader ExecuteReader(string cmdText) => this.ExecuteReader(cmdText, null);

        public IDataReader ExecuteReader(string cmdText, object param) => this.ExecuteReader(CommandType.StoredProcedure, cmdText, param);
        public IDataReader ExecuteReader(CommandType type, string cmdText, object param)
        {
            try
            {
                return _connection.ExecuteReader(cmdText, param, _transaction, null, type);
            }
            catch (Exception ex)
            {
                throw new DapperException(cmdText, ex.Message, param);
            }
            finally
            {

            }
        }

        public DataSet GetDataSet(string cmdText) => this.GetDataSet(cmdText, null);

        public DataSet GetDataSet(string cmdText, object param) => GetDataSet(CommandType.Text, cmdText, param);

        public abstract DataSet GetDataSet(CommandType type, string cmdText, object param);



        /// <summary>
        /// 获取存储过程值
        /// </summary>
        /// <typeparam name="TProcedure"></typeparam>
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        protected void GetProcedureParameters<TProcedure>(TProcedure procedure, out DynamicParameters parameters) where TProcedure : IProcedure
        {
            parameters = new DynamicParameters();
            foreach (PropertyInfo property in typeof(TProcedure).GetType().GetProperties())
            {
                string name = property.Name;
                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();
                if (column != null) name = column.Name;
                object value = property.GetValue(procedure);
                if (value == null) continue;
                parameters.Add(name, value);
            }
        }

        protected void GetDeleteAudited<TEntity>(TEntity entity) where TEntity : IEntity
        {
            if (entity is ISoftDelete)
            {
                ISoftDelete audited = entity.As<ISoftDelete>();
                audited.IsDeleted = true;
                audited.DeleteTime = DateTime.Now;
            }
        }
        protected void GetInsertAudited<TEntity>(TEntity entity) where TEntity : IEntity
        {
            if (entity is ICreationTime)
            {
                ICreationTime audited = entity.As<ICreationTime>();
                audited.CreateTime = DateTime.Now;
            }
        }
        protected void GetUpdateAudited<TEntity>(TEntity entity) where TEntity : IEntity
        {
            if (entity is ILastModifyTime)
            {
                ILastModifyTime audited = entity.As<ILastModifyTime>();
                audited.LastModifyTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 获取表达式解析对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IExpressionVisitor GetExpressionVisitor(Expression expression, DatabaseType type)
        {
            IExpressionVisitor visitor = null;
            switch (type)
            {
                case DatabaseType.SqlServer:
                    visitor = new SqlServerExpressionVisitor(expression);
                    break;
                case DatabaseType.MySql:
                    visitor = new MySqlExpressionVisitor(expression);
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.Sqlite:
                    visitor = new SqliteExpressionVisitor(expression);
                    break;
                default:
                    break;
            }
            return visitor;
        }

        #endregion


        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
