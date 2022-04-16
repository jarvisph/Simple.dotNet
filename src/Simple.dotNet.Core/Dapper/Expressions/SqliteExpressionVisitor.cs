using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Simple.Core.Extensions;

namespace Simple.Core.Dapper.Expressions
{
    public class SqliteExpressionVisitor : DapperExpressionVisitor, IExpressionVisitor
    {
        public SqliteExpressionVisitor()
        {

        }
        public SqliteExpressionVisitor(Expression expression)
        {
            this.Visit(expression);
        }
        protected override void AppendParameter(MemberExpression node, Stack<MemberInfo> members)
        {
            members.Push(node.Member);
            switch (node.Expression.NodeType)
            {
                case ExpressionType.Constant:
                    this.AppendParameter((ConstantExpression)node.Expression, members);
                    break;
                case ExpressionType.MemberAccess:
                    this.AppendParameter((MemberExpression)node.Expression, members);
                    break;
                case ExpressionType.Parameter:
                    ColumnAttribute column = node.Member.GetAttribute<ColumnAttribute>();
                    string field = column == null ? node.Member.Name : column.Name;
                    _field.Push($"[{field}]");
                    break;
                default:
                    break;
            }
        }

        protected override void AppendParameter(ConstantExpression node, Stack<MemberInfo> members)
        {
            object value = node.Value;
            foreach (var item in members)
            {
                switch (item.MemberType)
                {
                    case MemberTypes.Field:
                        value = ((FieldInfo)item).GetValue(value) ?? string.Empty;
                        break;
                    case MemberTypes.Property:
                        value = ((PropertyInfo)item).GetValue(value) ?? string.Empty;
                        break;
                    default:
                        break;
                }
            }
            if (value != null)
            {
                this.AppendParameter(value);
            }
        }
    }
}
