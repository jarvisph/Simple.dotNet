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


            //query��ƴ�Ӳ�ѯ��䣬û�н�����ʵ��ѯ
            var query = client.Query<UserESModel>().Where(c => c.ID == userId).Where(c => c.Money != 0);

            //��ȡһ������
            var model = query.FirstOrDefault();

            //��ȡ��������
            var list = query.ToList();

            //��ҳ��ȡ���� ����
            var desc = query.OrderByDescending(c => c.CreateAt).Paged(1, 20, out long total);

            //����
            var asc = query.OrderBy(c => c.CreateAt).Paged(1, 20, out total);

            //��ѯ��Ա�Ƿ����
            bool exists = client.Any<UserESModel>(t => t.ID == userId);

            //��ѯ��Ա��������
            int count = client.Count<UserESModel>(t => t.ID == userId);

            //�޸�ʵ���ֶ�
            client.Update(user, c => c.ID == userId, c => new
            {
                c.Money
            });
            //�޸�ĳ���ֶ�
            client.Update<UserESModel, decimal>(c => c.Money, 100, c => c.ID == userId);

            client.Delete<UserESModel>(c => c.ID == userId);
        }
    }
}
