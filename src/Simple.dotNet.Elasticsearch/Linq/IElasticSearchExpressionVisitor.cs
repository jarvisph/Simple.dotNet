using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    internal interface IElasticSearchExpressionVisitor<TDocument> : IDisposable where TDocument : class
    {
        QueryContainer Visit(Expression node);
        SortDescriptor<TDocument> Cell(Expression node);
    }
}
