using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Core.Expressions
{
    internal class GroupByExpressionVisitor : ExpressionVisitorBase, IDisposable
    {
        protected readonly Stack<MemberExpression> _group = new Stack<MemberExpression>();

        public new IEnumerable<MemberExpression> Visit(Expression node)
        {
            base.Visit(node);
            while (_group.Count > 0)
            {
                yield return _group.Pop();
            }
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            _group.Push(node);
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            foreach (var item in node.Arguments)
            {
                base.Visit(item);
            }
            return node;
        }
        public void Dispose()
        {
            _group.Clear();
            _field.Clear();
            _value.Clear();
        }
    }
}
