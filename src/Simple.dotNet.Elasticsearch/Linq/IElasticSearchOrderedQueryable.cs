using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.dotNet.Elasticsearch.Linq
{
    public interface IElasticSearchOrderedQueryable<TDocument> : IElasticSearchQueryable<TDocument> where TDocument : class
    {
       
    }
}
