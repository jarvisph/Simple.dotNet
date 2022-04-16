using Simple.Elasticsearch;
using Simple.Elasticsearch.Linq;
using Simple.Elasticsearch.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ElasticSearch.Test.DbContext
{
    public class ElasticSearchDbContext : ElasticSearchContext
    {
        public ElasticSearchDbContext() : base("http://localhost:9200")
        {

        }

        public ESSet<UserESModel> Order { get; set; }
    }
}
