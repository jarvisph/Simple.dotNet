using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRediSearch;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Redis.Test.RedisSearch
{

    [TestClass]
    public class Program
    {
        private Client _rediSearchClient;
        private IConnectionMultiplexer _connectionMultiplexer;
        [TestMethod]
        public void Main()
        {
            string conn = "192.168.0.176:6379";
            _connectionMultiplexer = ConnectionMultiplexer.Connect(conn);
            IDatabase db = _connectionMultiplexer.GetDatabase();
            _rediSearchClient = new Client("index_orders", db);
            Schema schema = CreateSchema();
            _rediSearchClient.CreateIndex(schema, new Client.ConfiguredIndexOptions(Client.IndexOptions.Default));

            Document document = new Document("110", new Dictionary<string, RedisValue>
            {
                {"CustomerID","n1231231".ToRedisValue() },
                {"CustomerName","商品".ToRedisValue() },
                {"OrderID",Guid.NewGuid().ToString("N").ToRedisValue() },
                {"City","深圳市".ToRedisValue() },
                {"PostalCode","ABC".ToRedisValue() },
                {"ExtendedPrice",128.5.ToRedisValue() },
                {"ProductName","苹果手机".ToRedisValue() }
            });


            _rediSearchClient.AddDocument(document);

            //_rediSearchClient.DeleteDocument("1");

            //_rediSearchClient.UpdateDocument("1", new Dictionary<string, RedisValue> { });
            //SearchResult search = _rediSearchClient.Search(new Query("") { WithPayloads = true });
            //search.Documents.Select(c => new
            //{
            //    ID = c.Id,
            //    score = c.Score,
            //    Name = (string)c["Name"]
            //});


        }
        private Schema CreateSchema()
        {
            Schema schema = new Schema();
            schema.AddTextField("CustomerID", 1)
                .AddTextField("CustomerName", 2)
                .AddTextField("OrderID", 1)
                .AddTextField("City", 1)
                .AddTextField("PostalCode", 1)
                .AddNumericField("ExtendedPrice")
                .AddTextField("ProductName", 1);
            return schema;
        }

    }
}
