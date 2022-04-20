using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Simple.Core.Extensions;

namespace Simple.Core.Data.Expressions
{
    public class SqlServerExpressionVisitor : SqlExpressionVisitorBase, ISqlExpressionVisitor
    {
        public SqlServerExpressionVisitor(Expression expression) : base(expression)
        {

        }

        public string GetSqlText(out DynamicParameters parameters, out Type type)
        {
            throw new NotImplementedException();
        }
    }
}
