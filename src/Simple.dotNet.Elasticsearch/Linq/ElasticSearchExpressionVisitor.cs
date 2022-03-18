using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Nest;
using Simple.dotNet.Core.Extensions;
using Simple.dotNet.Core.Expressions;
using System.Reflection;

namespace Simple.Elasticsearch.Linq
{
    /// <summary>
    /// ES 表达式树解析
    /// </summary>
    internal class ElasticSearchExpressionVisitor<TDocument> : ExpressionVisitorBase, IElasticSearchExpressionVisitor<TDocument> where TDocument : class
    {

        private readonly Stack<QueryContainer> _query = new Stack<QueryContainer>();
        private readonly Stack<SortDescriptor<TDocument>> _sort = new Stack<SortDescriptor<TDocument>>();
        private readonly Stack<object> _value = new Stack<object>();
        private readonly Stack<Expression> _field = new Stack<Expression>();

        public ElasticSearchExpressionVisitor()
        {

        }


        public new QueryContainer Visit(Expression node)
        {
            base.Visit(node);
            var query = _query.Pop();
            return query;
        }

        public SortDescriptor<TDocument> Cell(Expression node)
        {
            base.Visit(node);
            var sort = _sort.Pop();
            return sort;
        }

        public ElasticSearchExpressionVisitor(Expression expression)
        {
            this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _value.Push(node.Value);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Call:
                    VisitMember((MemberExpression)node.Object);
                    break;
            }
            foreach (Expression expression in node.Arguments)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Constant:
                        this.VisitConstant((ConstantExpression)expression);
                        break;
                }
            }
            object value = _value.Pop();
            Expression field = _field.Pop();
            if (value == null) return node;
            switch (node.Method.Name)
            {
                case "Contains":
                    _query.Push(new QueryContainerDescriptor<TDocument>().Wildcard(field, $"*{value}*"));
                    break;
                case "StartsWith":
                    _query.Push(new QueryContainerDescriptor<TDocument>().Wildcard(field, $"{value}*"));
                    break;
                case "EndsWith":
                    _query.Push(new QueryContainerDescriptor<TDocument>().Wildcard(field, $"*{value}"));
                    break;
                default:
                    break;
            }
            return node;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            base.VisitBinary(node);
            object value = null;
            Expression field = null;
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    field = _field.Pop();
                    value = _value.Pop();
                    break;
            }
            if (value == null) return node;
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    _query.Push(new QueryContainerDescriptor<TDocument>().Term(t => t.Field(field).Value(value)));
                    break;
                case ExpressionType.GreaterThan:
                    _query.Push(new QueryContainerDescriptor<TDocument>().LongRange(t => t.Field(field).GreaterThan(value.ToValue<long>())));
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _query.Push(new QueryContainerDescriptor<TDocument>().LongRange(t => t.Field(field).GreaterThanOrEquals(value.ToValue<long>())));
                    break;
                case ExpressionType.LessThan:
                    _query.Push(new QueryContainerDescriptor<TDocument>().LongRange(t => t.Field(field).LessThan(value.ToValue<long>())));
                    break;
                case ExpressionType.LessThanOrEqual:
                    _query.Push(new QueryContainerDescriptor<TDocument>().LongRange(t => t.Field(field).LessThanOrEquals(value.ToValue<long>())));
                    break;
                case ExpressionType.NotEqual:
                    _query.Push(new QueryContainerDescriptor<TDocument>().Bool(b => b.MustNot(t => t.Term(f => f.Field(field).Value(value)))));
                    break;
            }
            return node;
        }
        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            switch (node.Expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.Constant:
                    this.VisitConstant((ConstantExpression)node.Expression, node.Member);
                    break;
                case ExpressionType.Parameter:
                    _field.Push(node);
                    break;
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node, MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    _value.Push(((FieldInfo)member).GetValue(node.Value));
                    break;
                case MemberTypes.Property:
                    _value.Push(((PropertyInfo)member).GetValue(node.Value));
                    break;
            }
            return node;
        }
        public void Dispose()
        {
            _query.Clear();
            _field.Clear();
            _value.Clear();
        }


    }
}
