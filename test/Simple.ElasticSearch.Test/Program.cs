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

            //����
            client.Insert(new UserOrder { CreateTime = DateTime.Now, Money = 100, OrderID = "123456789", UserID = 1 });
            //��ѯ��Ա1�Ĳ��ҽ�����100�Ķ�����query��ƴ�Ӳ�ѯ��䣬û�н�����ʵ��ѯ
            var query = client.Query<UserOrder>(c => c.Where(1, t => t.UserID),
                                                c => c.Where(100, t => t.Money, ExpressionType.GreaterThanOrEqual));
            //�˴�����ʵ��ѯ����
            var list = client.Search(query).Documents.ToList();

            //���ݶ���ID��ѯ
            var order = client.FirstOrDefault<UserOrder>(c => c.Where("123456789", t => t.OrderID));

            //��ѯ�����Ƿ����
            bool exists = client.Any<UserOrder>(c => c.Where("123456789", t => t.OrderID));

            //��ѯ��Ա��������
            int count = client.Count<UserOrder>(c => c.Where(1, t => t.UserID));

        }
    }
}
