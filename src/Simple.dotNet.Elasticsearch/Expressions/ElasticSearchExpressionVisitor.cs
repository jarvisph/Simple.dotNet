using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Nest;
using Simple.dotNet.Core.Extensions;
using Simple.dotNet.Core.Expressions;
using System.Reflection;

namespace Simple.Elasticsearch.Expressions
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
        private readonly SortDescriptor<TDocument> _sort = new SortDescriptor<TDocument>();

        private List<Tuple<string, string, DateInterval?>>? _temas;
        private List<Tuple<string, string, Type>>? _select;

        public string? Cell { get; private set; }
        public List<string> Cells { get; private set; }

        public NewExpression? NewExpression { get; private set; }
        public Type Type { get; private set; }
        public ElasticSearchExpressionVisitor()
        {
            this.Cells = new List<string>();
            this.Type = typeof(TDocument);
        }
        public ElasticSearchExpressionVisitor(Expression expression) : this()
        {
            base.Visit(expression);
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        /// <returns></returns>
        public QueryContainer Query()
        {
            if (_query.Count == 0) return new QueryContainer();
            var query = _query.Pop();
            while (_query.Count > 0)
            {
                query &= _query.Pop();
            }
            return query;
        }

        public SortDescriptor<TDocument> Sort()
        {
            return _sort;
        }
        public AggregationContainerDescriptor<TDocument> Aggregation(out List<Tuple<string, string, Type>>? select)
        {
            select = _select;
            if (_select != null)
            {
                foreach (var item in _select)
                {
                    switch (item.Item2)
                    {
                        case "Sum":
                            _aggs.Sum(item.Item1, t => t.Field(item.Item1));
                            break;
                        case "Max":
                            _aggs.Max(item.Item1, t => t.Field(item.Item1));
                            break;
                        case "Min":
                            _aggs.Min(item.Item1, t => t.Field(item.Item1));
                            break;
                        case "Average":
                            _aggs.Average(item.Item1, t => t.Field(item.Item1));
                            break;
                    }
                }
            }
            if (_temas == null || _temas.Count == 0)
            {
                return _aggs;
            }
            else
            {
                var date_histogram = _temas.Where(c => c.Item2 == "date").ToArray();
                var terms = _temas.Where(c => c.Item2 == "terms").ToArray();
                string[] array = terms.Select(c => $"doc['{c.Item1}'].value").ToArray();
                string scriptname = string.Join("_", terms.Select(c => c.Item1));
                string script = string.Join("+'-'+", array);
                if (date_histogram.Length > 0)
                {
                    if (date_histogram[0].Item3 == null) throw new ElasticSearchException(nameof(DateInterval));
                    DateInterval interval = date_histogram[0].Item3.Value;
                    if (terms.Length > 0)
                    {
                        return new AggregationContainerDescriptor<TDocument>().DateHistogram(date_histogram[0].Item1, d => d.Interval(interval).Field(date_histogram[0].Item1).Aggregations(a => a.Terms(scriptname, t => t.Script(script).Aggregations(aggs => _aggs))));
                    }
                    else
                    {
                        return new AggregationContainerDescriptor<TDocument>().DateHistogram(date_histogram[0].Item1, d => d.Interval(interval).Aggregations(a => _aggs));
                    }
                }
                else
                {
                    return new AggregationContainerDescriptor<TDocument>().Terms(string.Join("_", _temas.Select(c => c.Item1)), t => t.Script(string.Join("+'-'+", array)).Size(1_000_000).Aggregations(aggs => _aggs));
                }
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
                default:
                    if (node.Type.IsGenericType)
                    {
                        this.Type = node.Type.GenericTypeArguments[0];
                    }
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
                    case ExpressionType.Quote:
                        switch (node.Method.Name)
                        {
                            case "GroupBy":
                                _temas = new ElasticSearchGroupExpressionVisitor().Visit(expression).ToList();
                                break;
                            default:
                                base.Visit(expression);
                                break;
                        }
                        break;
                    default:
                        base.Visit(expression);
                        break;
                }
            }
            this.Cell = node.Method.Name;
            this.Cells.Add(node.Method.Name);
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
                case "Sum":
                case "Max":
                case "Min":
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
                            case "Average":
                                _aggs.Average(member.Member.Name, t => t.Field(field));
                                break;
                        }
                    }
                    break;
                case "OrderBy":
                    _sort.Ascending(_field.Pop());
                    break;
                case "OrderByDescending":
                    _sort.Descending(_field.Pop());
                    break;
                default:
                    break;
            }
            return node;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            base.VisitBinary(node);
            object? value = null;
            Expression? field = null;
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
            switch (node.NodeType)
            {
                case ExpressionType.New:
                    _select = new ElasticSearchSelectExpressionVisitor().Visit(node).ToList();
                    break;
            }
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null) throw new ArgumentNullException("MemberExpression");
            if (node.Expression == null)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        switch (node.Member.Name)
                        {
                            case "Now":
                                _value.Push(DateTime.Now.GetTimestamp());
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            else
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
            _temas?.Clear();
            Cells.Clear();
        }


    }
}
