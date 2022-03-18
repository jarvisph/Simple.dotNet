using Nest;
using Simple.dotNet.Core.Mapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    /// <summary>
    /// ES Queryable
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public class ElasticSearchQueryable<TDocument> : IElasticSearchQueryable<TDocument>, IElasticSearchOrderedQueryable<TDocument>, IQueryProvider where TDocument : class, IDocument
    {
        private Expression _expression;

        public IElasticClient Client { get; }
        public QueryContainer Query { get; private set; }
        public SortDescriptor<TDocument> Sort { get; private set; }

        public ElasticSearchQueryable(IElasticClient client)
        {
            _expression = Expression.Constant(this);
            Query = new QueryContainer();
            Sort = new SortDescriptor<TDocument>();
            Client = client;
        }

        public Type ElementType => typeof(TDocument);

        public Expression Expression => _expression;

        public string IndexName => typeof(TDocument).GetIndexName();

        public IQueryProvider Provider => this;


        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(ElasticSearchQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement>? CreateQuery<TElement>(Expression expression)
        {
            _expression = expression;
            return default;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            object document = Client.FirstOrDefault<TDocument>(Query);
            return (TResult)document;
        }

        public IElasticSearchQueryable<TDocument> Where(Expression<Func<TDocument, bool>> expression)
        {
            using (IElasticSearchExpressionVisitor<TDocument> visitor = new ElasticSearchExpressionVisitor<TDocument>())
            {
                Query = Query && visitor.Visit(expression);
            }
            return this;
        }
        public IElasticSearchOrderedQueryable<TDocument> OrderByDescending<TKey>(Expression<Func<TDocument, TKey>> keySelector)
        {
            Sort.Descending(keySelector);
            return this;
        }

        public IElasticSearchOrderedQueryable<TDocument> OrderBy<TKey>(Expression<Func<TDocument, TKey>> keySelector)
        {
            Sort.Ascending(keySelector);
            return this;
        }

        public IEnumerator<TDocument> GetEnumerator()
        {
            return Client.GetAll<TDocument>(Query).GetEnumerator();
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }
}
