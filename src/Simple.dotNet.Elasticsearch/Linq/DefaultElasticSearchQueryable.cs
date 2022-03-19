using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    internal class DefaultElasticSearchQueryable<T> : IQueryable<T>
    {
        private readonly Expression _expression;
        private readonly IQueryProvider _provider;
        private readonly IElasticClient _client;
        public DefaultElasticSearchQueryable(Expression expression, IQueryProvider provider, IElasticClient client)
        {
            _expression = expression;
            _provider = provider;
            _client = client;

        }
        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            using (IElasticSearchExpressionVisitor visitor = new ElasticSearchExpressionVisitor())
            {

            }
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
