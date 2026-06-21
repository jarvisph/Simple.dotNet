using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Simple.Core.Domain.Dto.Page;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Core.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int page, int limit)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (page <= 0) page = 1;
            if (limit <= 0) limit = 10;
            int skip = (page - 1) * limit;
            return query.Skip(skip).Take(limit);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> query, object value, Expression<Func<T, bool>> predicate)
        {
            if (value == null) return query;
            else if (string.IsNullOrWhiteSpace(value.ToString())) return query;
            return query.Where(predicate);
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> Where<T, TValue>(this IQueryable<T> query, TValue? value, Expression<Func<T, bool>> predicate) where TValue : struct
        {
            if (value.HasValue) return query.Where(predicate);
            return query;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> expression, string field, string order)
        {
            // 验证参数
            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(order))
                return query.OrderByDescending(expression);

            // 解析排序方向
            var isAscending = order.Trim().Equals("ASC", StringComparison.OrdinalIgnoreCase);
            var methodName = isAscending ? "OrderBy" : "OrderByDescending";

            // 获取属性信息
            var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return query.OrderByDescending(expression);

            // 构建表达式树
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            // 调用排序方法
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
        /// <summary>
        /// 倒序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> expression, string field, string order)
        {
            return (IOrderedQueryable<T>)query.Sort(expression, field, order);
        }
        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> expression, string field, string order)
        {
            return (IOrderedQueryable<T>)query.Sort(expression, field, order);
        }
    }
}
