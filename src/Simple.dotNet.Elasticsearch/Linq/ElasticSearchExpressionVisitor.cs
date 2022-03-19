﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Nest;
using Simple.dotNet.Core.Extensions;
using Simple.dotNet.Core.Expressions;
using System.Reflection;
using Simple.dotNet.Core.Dapper.Expressions;

namespace Simple.Elasticsearch.Linq
{
    /// <summary>
    /// ES 表达式树解析
    /// </summary>
    internal class ElasticSearchExpressionVisitor<TDocument> : ExpressionVisitorBase, IElasticSearchExpressionVisitor<TDocument> where TDocument : class
    {
        private readonly Stack<QueryContainer> _query = new Stack<QueryContainer>();
        private readonly Stack<object> _value = new Stack<object>();
        private readonly Stack<Expression> _field = new Stack<Expression>();
        private readonly AggregationContainerDescriptor<TDocument> _aggs = new AggregationContainerDescriptor<TDocument>();
        private readonly List<string> _temas = new List<string>();

        public string? Cell { get; private set; }

        public ElasticSearchExpressionVisitor()
        {

        }
        public ElasticSearchExpressionVisitor(Expression expression) : this()
        {
            base.Visit(expression);
        }

        public QueryContainer Query(Expression node)
        {
            base.Visit(node);
            if (_query.Count == 0) return new QueryContainer();
            var query = _query.Pop();
            while (_query.Count > 0)
            {
                query &= _query.Pop();
            }
            return query;
        }
        public AggregationContainerDescriptor<TDocument> Group()
        {
            if (_temas.Count > 0)
            {
                string[] array = _temas.Select(c => $"doc['{c}'].value").ToArray();
                return new AggregationContainerDescriptor<TDocument>().Terms(string.Join("_", _temas), t => t.Script(string.Join("+'-'+", array)).Size(1_000_000).Aggregations(aggs => _aggs));
            }
            else
            {
                return _aggs;
            }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            switch (node.Type.Name)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "Boolean":
                case "String":
                case "Decimal":
                case "Single":
                case "Double":
                case "DateTime":
                case "Byte":
                    _value.Push(node.Value);
                    break;
            }
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Call:
                    if (node.Object != null)
                    {
                        VisitMember((MemberExpression)node.Object);
                    }
                    break;
            }
            foreach (Expression expression in node.Arguments)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Constant:
                        this.VisitConstant((ConstantExpression)expression);
                        break;
                    default:
                        base.Visit(expression);
                        break;
                }
            }

            switch (node.Method.Name)
            {
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    {
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
                        }
                    }
                    break;
                case "GroupBy":
                    {
                        this.Cell = node.Method.Name;
                        if (_field.Count > 0)
                        {
                            while (_field.Count > 0)
                            {
                                var member = (MemberExpression)_field.Pop();
                                _temas.Add(member.Member.Name);
                            }
                        }
                    }
                    break;
                case "Sum":
                case "Max":
                case "Min":
                case "Count":
                case "Average":
                    {
                        Expression field = _field.Pop();
                        var member = (MemberExpression)field;
                        switch (node.Method.Name)
                        {
                            case "Sum":
                                _aggs.Sum(member.Member.Name, t => t.Field(field));
                                break;
                            case "Max":
                                _aggs.Max(member.Member.Name, t => t.Field(field));
                                break;
                            case "Min":
                                _aggs.Min(member.Member.Name, t => t.Field(field));
                                break;
                            case "Count":
                                _aggs.ValueCount(member.Member.Name, t => t.Field(field));
                                break;
                            case "Average":
                                _aggs.Average(member.Member.Name, t => t.Field(field));
                                break;
                        }
                    }
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
            if (node == null) throw new ArgumentNullException("MemberExpression");
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
            _temas.Clear();
        }
    }

    internal class ElasticSearchExpressionVisitor : ExpressionVisitorBase, IElasticSearchExpressionVisitor
    {

        public void Dispose()
        {

        }
    }
}
