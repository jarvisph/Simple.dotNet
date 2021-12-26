using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Core.Dapper
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    public abstract class DapperRepositoryBase : DapperDatabase, IWriteRepository, IProcedureRepository
    {
        protected DapperRepositoryBase(string connectionString, IsolationLevel isolationLevel, DatabaseType type) : base(connectionString, isolationLevel, type)
        {

        }
        public abstract bool Any<TEntity>() where TEntity : IEntity;
        public abstract bool Any<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        public abstract int Count<TEntity>() where TEntity : IEntity;
        public abstract int Count<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        public abstract bool Delete<TEntity>(TEntity entity) where TEntity : IEntity;
        public abstract bool Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;

        public abstract TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        public abstract TValue FirstOrDefault<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> value) where TEntity : IEntity;

        public abstract IEnumerable<TEntity> GetAll<TEntity>() where TEntity : IEntity;
        public abstract IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        public abstract IQueryable<TEntity> Query<TEntity>() where TEntity : IEntity;

        public abstract bool Insert<TEntity>(TEntity entity) where TEntity : IEntity;
        public abstract bool InsertIdentity<TEntity>(TEntity entity) where TEntity : IEntity;
        public abstract bool Update<TEntity, TValue>(TEntity entity, Expression<Func<TEntity, TValue>> field, TValue value) where TEntity : IEntity;
        public abstract bool Update<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value) where TEntity : IEntity;
        public abstract bool Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] fields) where TEntity : IEntity;
        public abstract bool Plus<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value)
         where TEntity : IEntity
         where TValue : struct;

        public abstract int Execute<TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
        public abstract T ExecuteScalar<T, TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
        public abstract IDataReader GetDataReader<TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
    }
}
