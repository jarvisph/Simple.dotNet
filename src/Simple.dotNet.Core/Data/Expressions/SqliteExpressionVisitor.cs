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
    internal class SqliteExpressionVisitor : SqlExpressionVisitorBase, ISqlExpressionVisitor
    {
        public SqliteExpressionVisitor()
        {

        }
        public SqliteExpressionVisitor(Expression expression)
        {
            this.Visit(expression);
        }

        public string GetSqlText(out DynamicParameters parameters, out Type type)
        {
            throw new NotImplementedException();
        }
    }
}
