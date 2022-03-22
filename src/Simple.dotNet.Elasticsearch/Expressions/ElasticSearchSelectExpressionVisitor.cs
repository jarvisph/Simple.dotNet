using Simple.dotNet.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Expressions
{
    internal class ElasticSearchSelectExpressionVisitor : ExpressionVisitorBase
    {
        private readonly Queue<MemberExpression> _field = new Queue<MemberExpression>();
        private readonly Queue<Tuple<string, string, Type>> _select = new Queue<Tuple<string, string, Type>>();
        /// <summary>
        /// 获取Select字段类型属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Item1=字段，Item2=Call类型，Item3=字段类型Type</returns>
        public new IEnumerable<Tuple<string, string, Type>> Visit(Expression node)
        {
            base.Visit(node);
            while (_select.Count > 0)
            {
                yield return _select.Dequeue();
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _field.Enqueue(node);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            foreach (Expression expression in node.Arguments)
            {
                base.Visit(expression);
            }
            if (_field.Count > 0)
            {
                while (_field.Count > 0)
                {
                    MemberExpression member = _field.Dequeue();
                    switch (node.Method.Name)
                    {
                        case "Sum":
                        case "Max":
                        case "Min":
                        case "Average":
                            _select.Enqueue(new Tuple<string, string, Type>(member.Member.Name, node.Method.Name, member.Type));
                            break;
                        case "ToDateTime":
                            _select.Enqueue(new Tuple<string, string, Type>(member.Member.Name, "DateTime", typeof(DateTime)));
                            break;
                        default:
                            _select.Enqueue(new Tuple<string, string, Type>(member.Member.Name, "Key", member.Type));
                            break;
                    }
                }
            }
            else
            {
                switch (node.Method.Name)
                {
                    case "Count":
                        _select.Enqueue(new Tuple<string, string, Type>(node.Method.Name, node.Method.Name, node.Type));
                        break;
                }
            }

            return node;
        }
    }
}
