using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Simple.Elasticsearch.Linq
{
    public interface IElasticSearchGroupingQueryable<TDocument> where TDocument : class
    {
        TDocument FirstOrDefault();
    }
}
