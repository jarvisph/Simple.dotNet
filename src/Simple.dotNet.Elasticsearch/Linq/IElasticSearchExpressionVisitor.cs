using Nest;
using Simple.dotNet.Core.Dapper.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    internal interface IElasticSearchExpressionVisitor<TDocument> : IDisposable where TDocument : class
    {
        QueryContainer Query(Expression node);
        AggregationContainerDescriptor<TDocument> Aggregation();
        string? Cell { get; }
        List<string> Cells { get; }

    }
    internal interface IElasticSearchExpressionVisitor : IDisposable
    {

    }
}
