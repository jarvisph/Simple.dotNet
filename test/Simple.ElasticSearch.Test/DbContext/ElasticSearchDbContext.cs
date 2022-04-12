using Simple.dotNet.Elasticsearch;
using Simple.dotNet.Elasticsearch.Linq;
using Simple.dotNet.Elasticsearch.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.ElasticSearch.Test.DbContext
{
    public class ElasticSearchDbContext : ElasticSearchContext
    {
        public ElasticSearchDbContext() : base("http://localhost:9200")
        {

        }

        public ESSet<UserESModel> Order { get; set; }
    }
}
