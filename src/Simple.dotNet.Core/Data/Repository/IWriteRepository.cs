using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Core.Data.Repository
{
    /// <summary>
    /// 可写 仓储
    /// </summary>
    public interface IWriteRepository : IReadRepository
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Insert<TEntity>(TEntity entity) where TEntity : IEntity;
        bool InsertIdentity<TEntity>(TEntity entity) where TEntity : IEntity;

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        bool Delete<TEntity>(TEntity entity) where TEntity : IEntity;
        bool Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update<TEntity, TValue>(TEntity entity, Expression<Func<TEntity, TValue>> field, TValue value) where TEntity : IEntity;
        bool Update<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value) where TEntity : IEntity;
        bool Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] fields) where TEntity : IEntity;
        /// <summary>
        /// 加操作
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Plus<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> field, TValue value) where TEntity : IEntity where TValue : struct;


    }
}
