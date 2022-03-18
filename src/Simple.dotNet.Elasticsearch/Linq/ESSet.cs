using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    public abstract class ESSet<TDocument> : IESRepository<TDocument> where TDocument : class, IDocument
    {
        private readonly IElasticClient _client;
        private QueryContainer _query;
        public ESSet()
        {
            _query = new QueryContainer();
        }
        private string _indexname
        {
            get
            {
                return typeof(TDocument).GetIndexName();
            }
        }
        public bool Any() => _client.Any<TDocument>();

        public bool Any(Expression<Func<TDocument, bool>> expression)
        {
            using (IElasticSearchExpressionVisitor<TDocument> visitor = new ElasticSearchExpressionVisitor<TDocument>())
            {
                _query = visitor.Visit(expression);
            }
            return (int)_client.Count<TDocument>(c => c.Index(_indexname).Query(q => _query)).Count > 0;
        }
        public int Count()
        {
            return (int)_client.Count<TDocument>(c => c.Index(_indexname)).Count;
        }
        public abstract int Count(Expression<Func<TDocument, bool>> expression);
        public abstract bool Delete(Expression<Func<TDocument, bool>> expression);
        public abstract TDocument FirstOrDefault();
        public abstract TDocument FirstOrDefault(Expression<Func<TDocument, bool>> expression);
        public abstract IQueryable<TDocument> GetAll();
        public abstract IQueryable<TDocument> GetAll(Expression<Func<TDocument, bool>> expression);

        public abstract bool Insert(TDocument entity);
        public abstract bool Insert(IEnumerable<TDocument> entities);
        public abstract bool Update(TDocument entity, Expression<Func<TDocument, bool>> expression);
        public abstract bool Update(TDocument entity, Expression<Func<TDocument, bool>> expression, Expression<Func<TDocument, bool>> fields);
        public abstract bool Update<TValue>(TValue value, Expression<Func<TDocument, TValue>> field, Expression<Func<TDocument, bool>> expression);
    }
}
