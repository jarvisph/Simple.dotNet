using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    public interface IElasticSearchQueryable<TDocument> : IQueryable<TDocument> where TDocument : class, IDocument
    {
        IElasticSearchQueryable<TDocument> Where(Expression<Func<TDocument, bool>> expression);
        IElasticSearchOrderedQueryable<TDocument> OrderByDescending<TKey>(Expression<Func<TDocument, TKey>> keySelector);
        IElasticSearchOrderedQueryable<TDocument> OrderBy<TKey>(Expression<Func<TDocument, TKey>> keySelector);
        QueryContainer Query { get; }
        IElasticClient Client { get; }
    }
}
