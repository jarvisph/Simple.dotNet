using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Core.Expressions
{
    internal class OrderByExpressionVisitory : ExpressionVisitorBase, IDisposable
    {
        private readonly Stack<MemberExpression> _order = new Stack<MemberExpression>();
        public new IEnumerable<MemberExpression> Visit(Expression node)
        {
            base.Visit(node);
            while (_order.Count > 0)
            {
                yield return _order.Pop();
            }
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            _order.Push(node);
            return node;
        }
        public void Dispose()
        {

        }
    }
}
