using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Expressions
{
    internal class SelectExpressionVisitor : ExpressionVisitorBase, IDisposable
    {
        private readonly Queue<MemberInfo> _members = new Queue<MemberInfo>();
        private readonly Queue<SelectProperty> _select = new Queue<SelectProperty>();

        public new IEnumerable<SelectProperty> Visit(Expression node)
        {
            NewExpression expression = node as NewExpression;
            foreach (var item in expression.Members)
            {
                _members.Enqueue(item);
            }
            base.Visit(node);
            while (_select.Count > 0)
            {
                yield return _select.Dequeue();
            }
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberInfo member = _members.Dequeue();
            _select.Enqueue(new SelectProperty(member, "Key", node.Type));
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MemberInfo member = _members.Dequeue();
            switch (node.Method.Name)
            {
                case "Sum":
                case "Max":
                case "Min":
                case "Average":
                case "Count":
                    _select.Enqueue(new SelectProperty(member, node.Method.Name, node.Type)); ;
                    break;
                case "ToDateTime":
                    _select.Enqueue(new SelectProperty(member, "DateTime", typeof(DateTime)));
                    break;
            }
            return node;
        }
        public void Dispose()
        {
            _members.Clear();
            _select.Clear();
        }
    }
}
