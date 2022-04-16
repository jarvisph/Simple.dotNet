using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Simple.Core.Data.Schema;

namespace Simple.Core.Data.Repository
{
    /// <summary>
    /// 可读仓储
    /// </summary>
    public interface IReadRepository
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        TValue FirstOrDefault<TEntity, TValue>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TValue>> value) where TEntity : IEntity;

        /// <summary>
        /// 所有数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : IEntity;
        IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
        IQueryable<TEntity> Query<TEntity>() where TEntity : IEntity;
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        bool Any<TEntity>() where TEntity : IEntity;
        bool Any<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        int Count<TEntity>() where TEntity : IEntity;
        int Count<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;

    }
}
