using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.dotNet.Core.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 获取Experssion字段名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetFieldName<TEntity, TValue>(this Expression<Func<TEntity, TValue>> exp)
        {
            MemberExpression node = exp.Body as MemberExpression;
            ColumnAttribute column = node.Member.GetAttribute<ColumnAttribute>();
            return column == null ? node.Member.Name : column.Name;
        }
        /// <summary>
        /// 获取PropertyInfo
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static PropertyInfo ToPropertyInfo<TEntity, TValue>(this Expression<Func<TEntity, TValue>> exp)
        {
            MemberExpression node = exp.Body as MemberExpression;
            if (node == null)
            {
                var visitor = new MemberAccesses(exp.Parameters[0]);
                visitor.Visit(exp);
                node = visitor.Member;
            }
            return (PropertyInfo)node.Member;
        }
        /// <summary>
        /// 获取表达式对应sql运算类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetExpressionType(this ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.OrElse:
                case ExpressionType.Or: return "OR";
                case ExpressionType.AndAlso:
                case ExpressionType.And: return "AND";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return string.Empty;
        }
    }
    internal class MemberAccesses : ExpressionVisitor
    {
        private ParameterExpression _parameter;
        public MemberExpression Member { get; private set; }

        public MemberAccesses(ParameterExpression parameter)
        {
            this._parameter = parameter;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _parameter)
            {
                Member = node;
            }
            return base.VisitMember(node);
        }
    }
}
