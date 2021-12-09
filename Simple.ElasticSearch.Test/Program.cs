using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Simple.Elasticsearch;
using System;
using Simple.ElasticSearch.Test.Model;
using System.Linq;
using System.Linq.Expressions;

namespace Simple.ElasticSearch.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            IElasticClient client = new ElasticClient(new Uri("localhost:9100"));

            //新增
            client.Insert(new UserOrder { CreateTime = DateTime.Now, Money = 100, OrderID = "123456789", UserID = 1 });
            //查询会员1的并且金额大于100的订单，query仅拼接查询语句，没有进行真实查询
            var query = client.Query<UserOrder>(c => c.Where(1, t => t.UserID),
                                                c => c.Where(100, t => t.Money, ExpressionType.GreaterThanOrEqual));
            //此处乃真实查询数据
            var list = client.Search(query).Documents.ToList();

            //根据订单ID查询
            var order = client.FirstOrDefault<UserOrder>(c => c.Where("123456789", t => t.OrderID));

            //查询订单是否存在
            bool exists = client.Any<UserOrder>(c => c.Where("123456789", t => t.OrderID));

            //查询会员订单总数
            int count = client.Count<UserOrder>(c => c.Where(1, t => t.UserID));

        }
    }
}
