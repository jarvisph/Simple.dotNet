using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Simple.dotNet.Core.Domain.Dto.Page;

namespace Simple.dotNet.Core.Extensions
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
        /// <param name="type"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string field, string type)
        {
            if (field.IsNullOrWhiteSpace() || type.IsNullOrWhiteSpace()) return query;
            string sorting = string.Empty;
            if (type.ToUpper().Trim() == "ASC")
            {
                sorting = "OrderBy";
            }
            else if (type.ToUpper().Trim() == "DESC")
            {
                sorting = "OrderByDescending";
            }
            ParameterExpression param = Expression.Parameter(typeof(T), field);
            PropertyInfo property = typeof(T).GetProperty(field);
            if (property == null) return query;
            Type[] types = new Type[2];
            types[0] = typeof(T);
            types[1] = property.PropertyType;
            Expression exp = Expression.Call(typeof(Queryable), sorting, types, query.Expression, Expression.Lambda(Expression.Property(param, field), param));
            return query.AsQueryable().Provider.CreateQuery<T>(exp);
        }
        /// <summary>
        /// 倒序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> expression, string field, string type)
        {
            if (field.IsNullOrWhiteSpace() || type.IsNullOrWhiteSpace()) return query.OrderByDescending(expression);
            return (IOrderedQueryable<T>)query.Sort(field, type);
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
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> expression, string field, string type)
        {
            if (field.IsNullOrWhiteSpace() || type.IsNullOrWhiteSpace()) return query.OrderBy(expression);
            return (IOrderedQueryable<T>)query.Sort(field, type);
        }
    }
}
