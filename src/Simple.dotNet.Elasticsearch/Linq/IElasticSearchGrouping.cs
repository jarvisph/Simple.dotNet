using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    public interface IElasticSearchGrouping<TKey, TDocument> where TDocument : class
    {
        TKey Key { get; }

        decimal Sum(Expression<Func<TDocument, decimal>> keySelector);
        decimal Max(Expression<Func<TDocument, decimal>> keySelector);
        decimal Min(Expression<Func<TDocument, decimal>> keySelector);
        int Count(Expression<Func<TDocument, int>> keySelector);

    }
}
