using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.dotNet.Core.Expressions
{
    public abstract class ExpressionVisitorBase : ExpressionVisitor
    {
        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            return node.Expression;
        }
        protected virtual Expression VisitConstant(ConstantExpression node, MemberInfo member)
        {
            return node;
        }


    }
}
