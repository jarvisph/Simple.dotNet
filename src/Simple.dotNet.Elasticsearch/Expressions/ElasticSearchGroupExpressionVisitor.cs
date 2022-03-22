using Nest;
using Simple.dotNet.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Expressions
{
    internal class ElasticSearchGroupExpressionVisitor : ExpressionVisitorBase
    {
        private readonly Stack<Tuple<string, string, DateInterval?>> _field = new Stack<Tuple<string, string, DateInterval?>>();
        public ElasticSearchGroupExpressionVisitor()
        {

        }
        /// <summary>
        /// 获取聚合内容
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Item1=字段，Item2=聚合类型，Item3=时间聚合类型</returns>
        public new IEnumerable<Tuple<string, string, DateInterval?>> Visit(Expression node)
        {
            base.Visit(node);
            while (_field.Count > 0)
            {
                yield return _field.Pop();
            }
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType.Name == "DateTime")
            {
                MemberExpression member = (MemberExpression)node.Expression;
                _field.Push(new Tuple<string, string, DateInterval?>(member.Member.Name, "date", node.Member.Name.ToEnum<DateInterval>()));
            }
            else
            {
                _field.Push(new Tuple<string, string, DateInterval?>(node.Member.Name, "terms", null));
            }
            return node;
        }
        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }
    }
}
