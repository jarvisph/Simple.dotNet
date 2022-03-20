using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Elasticsearch.Linq
{
    /// <summary>
    /// ES Queryable
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public class ElasticSearchQueryable<TDocument> : IElasticSearchQueryable<TDocument>, IElasticSearchOrderedQueryable<TDocument>, IElasticSearchGroupingQueryable<TDocument>, IQueryProvider where TDocument : class, IDocument
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

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DefaultElasticSearchQueryable<TElement>(expression, this, Client);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            object? value = null;
            using (IElasticSearchExpressionVisitor<TDocument> vistor = new ElasticSearchExpressionVisitor<TDocument>(expression))
            {
                if (vistor.Cells.Contains("GroupBy"))
                {
                    switch (vistor.Cell)
                    {
                        case "FirstOrDefault":
                            value = Client.GroupBy(Query, vistor.Aggregation());
                            break;
                        default:
                            throw new ElasticSearchException($"Not implemented {vistor.Cell}");
                    }
                }
                else
                {
                    switch (vistor.Cell)
                    {
                        case "Any":
                            value = Client.Any<TDocument>(Query);
                            break;
                        case "Count":
                            value = Client.Count<TDocument>(Query);
                            break;
                        case "FirstOrDefault":
                            value = Client.FirstOrDefault<TDocument>(Query);
                            break;
                        case "Max":
                            value = Client.Max<TDocument, TResult>(Query, vistor.Aggregation());
                            break;
                        case "Min":
                            value = Client.Min<TDocument, TResult>(Query, vistor.Aggregation());
                            break;
                        case "Average":
                            value = Client.Average<TDocument, TResult>(Query, vistor.Aggregation());
                            break;
                        default:
                            throw new ElasticSearchException($"Not implemented {vistor.Cell}");
                    }
                }
            }
            return (TResult)value;
        }

        public IElasticSearchQueryable<TDocument> Where(Expression<Func<TDocument, bool>> expression)
        {
            using (IElasticSearchExpressionVisitor<TDocument> visitor = new ElasticSearchExpressionVisitor<TDocument>())
            {
                Query = Query && visitor.Query(expression);
            }
            return this;
        }
        public IElasticSearchQueryable<TDocument> Where<TValue>(TValue? value, Expression<Func<TDocument, bool>> expression) where TValue : struct
        {
            if (value == null) return this;
            using (IElasticSearchExpressionVisitor<TDocument> visitor = new ElasticSearchExpressionVisitor<TDocument>())
            {
                Query = Query && visitor.Query(expression);
            }
            return this;
        }

        public IElasticSearchQueryable<TDocument> Where(object value, Expression<Func<TDocument, bool>> expression)
        {
            if (value == null) return this;
            using (IElasticSearchExpressionVisitor<TDocument> visitor = new ElasticSearchExpressionVisitor<TDocument>())
            {
                Query = Query && visitor.Query(expression);
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

        public IElasticSearchGroupingQueryable<IElasticSearchGrouping<TKey, TDocument>> GroupBy<TKey>(Expression<Func<TDocument, TKey>> keySelector)
        {
            throw new NotImplementedException();
        }

        public IElasticSearchGroupingQueryable<TDocument> Select<TKey>(Expression<Func<TDocument, TKey>> keySelector)
        {
            throw new NotImplementedException();
        }

        public TDocument FirstOrDefault()
        {
            throw new NotImplementedException();
        }


    }
}
