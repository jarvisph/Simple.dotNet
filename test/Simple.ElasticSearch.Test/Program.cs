using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Simple.Elasticsearch;
using System;
using Simple.ElasticSearch.Test.Model;
using System.Linq;
using Elasticsearch.Net;

namespace Simple.ElasticSearch.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            var urls = "http://ES3-DEV:9200".Split(';').Select(http => new Uri(http));
            var staticConnectionPool = new StaticConnectionPool(urls);
            var settings = new ConnectionSettings(staticConnectionPool).DisableDirectStreaming().DefaultFieldNameInferrer(name => name);
            IElasticClient client = new ElasticClient(settings);

            UserESModel user = new UserESModel
            {
                ID = 10001,
                CreateAt = DateTime.Now,
                Money = 10000,
                UserName = "ceshi01"
            };

            int userId = 10001;

            user = client.FirstOrDefault<UserESModel>(t => t.UserName.Contains("ceshi"));

            user = client.FirstOrDefault<UserESModel>(t => t.ID == userId);

            user = client.FirstOrDefault<UserESModel>(t => t.ID == 10001);


            //query仅拼接查询语句，没有进行真实查询
            var query = client.Query<UserESModel>().Where(c => c.ID == userId).Where(c => c.Money != 0);

            //获取一条数据
            var model = query.FirstOrDefault();

            //获取所有数据
            var list = query.ToList();

            //分页获取数据 倒序
            var desc = query.OrderByDescending(c => c.CreateAt).Paged(1, 20, out long total);

            //升序
            var asc = query.OrderBy(c => c.CreateAt).Paged(1, 20, out total);

            //查询会员是否存在
            bool exists = client.Any<UserESModel>(t => t.ID == userId);

            //查询会员订单总数
            int count = client.Count<UserESModel>(t => t.ID == userId);

            //修改实体字段
            client.Update(user, c => c.ID == userId, c => new
            {
                c.Money
            });
            //修改某个字段
            client.Update<UserESModel, decimal>(c => c.Money, 100, c => c.ID == userId);

            client.Delete<UserESModel>(c => c.ID == userId);
        }
    }
}
