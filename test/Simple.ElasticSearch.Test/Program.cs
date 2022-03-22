using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Simple.Elasticsearch;
using System;
using Simple.ElasticSearch.Test.Model;
using System.Linq;
using Elasticsearch.Net;
using Simple.Elasticsearch.Linq;
using Simple.dotNet.Core.Extensions;

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
            //DefaultFieldNameInferrer ����ʵ���ֶ���ES�����ֶ�һֱ��ESĬ������ĸСд
            var settings = new ConnectionSettings(staticConnectionPool).DisableDirectStreaming().DefaultFieldNameInferrer(name => name);
            IElasticClient client = new ElasticClient(settings);

            string action = "Queryable";
            switch (action)
            {
                case "Queryable":
                    this.Queryable(client);
                    break;
                case "Insert":
                    this.Insert(client);
                    break;
                case "Delete":
                    this.Delete(client);
                    break;
                case "Update":
                    this.Update(client);
                    break;
                case "Select":
                    this.Select(client);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// ����Queryable��ѯ
        /// </summary>
        /// <param name="client"></param>
        public void Queryable(IElasticClient client)
        {
            int userId = 10001;
            int[] sites = new[] { 1000, 1001 };
            //query��ƴ�Ӳ�ѯ��䣬û�н�����ʵ��ѯ
            var query = client.Query<UserESModel>()
                                                  .Where(c => c.ID == userId)
                                                  .Where(c => c.Money != 0)
                                                  .Where(c => c.SiteID > 0)
                                                  .Where(DateTime.Now, c => c.CreateAt < DateTime.Now)
                                                  .Where(c => c.UserName.Contains("ceshi"))
                                                  .Where(c => !c.IsTest)
                                                  .Where(sites, c => !sites.Contains(c.SiteID))
                                                  .Where(null, t => t.UserName == null);
            //�Ƿ����
            bool exists = query.Any();
            //����
            int count = query.Count();
            //��������
            var user = query.FirstOrDefault();
            //���ֵ
            decimal max = query.Max(t => t.Money);
            //��Сֵ
            decimal min = query.Min(t => t.Money);
            //ƽ��ֵ
            decimal average = query.Average(t => t.Money);

            {
                //�������ۺ�
                var group = query.GroupBy(c => true).Select(c => new { Count = c.Count(), Money = c.Sum(t => t.Money) });

                var group_list = group.ToList();

                var group_firt = group.FirstOrDefault();
            }

            {
                //�����ۺ�
                var group = query.GroupBy(c => new { c.SiteID, c.ID }).Select(c => new { c.Key.SiteID, Money = c.Sum(t => t.Money), });

                var group_list = group.ToList();

                var group_firt = group.FirstOrDefault();
            }

            {
                //����+�����ۺ�
                var group = query.GroupBy(c => new { c.CreateAt.Month, c.SiteID }).Select(c => new
                {
                    CreateAt = c.Key.Month.ToDateTime(),
                    c.Key.SiteID,
                    Money = c.Sum(t => t.Money)
                });

                var group_list = group.ToList();

                var group_firt = group.FirstOrDefault();

            }

            //��ҳ��ȡ���� ����
            var desc = query.OrderByDescending(c => c.CreateAt).Paged(1, 20, out long total);

            //����
            var asc = query.OrderBy(c => c.CreateAt).Paged(1, 20, out total);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="client"></param>
        public void Insert(IElasticClient client)
        {
            UserESModel user = new UserESModel
            {
                ID = 10001,
                CreateAt = DateTime.Now,
                Money = 10000,
                UserName = "ceshi01"
            };
            //��������
            client.Insert(user);
            //��������
            client.Insert(new[] { user });
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="client"></param>
        public void Delete(IElasticClient client)
        {
            int userId = 10001;
            //ɾ��
            client.Delete<UserESModel>(c => c.ID == userId);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="client"></param>
        public void Update(IElasticClient client)
        {
            int userId = 10001;
            //�޸�ָ�����ֶΣ�PS������޸Ķ���ֶΣ�����ʹ��Insert��ES����������ڣ����޸�ȫ���ֶΣ�
            client.Update(new UserESModel
            {
                NickName = "����",
                Money = 10001
            }, c => c.ID == userId, c => new
            {
                c.Money,
                c.NickName
            });
            //�޸ĵ����ֶ�
            client.Update<UserESModel, decimal>(c => c.Money, 100, c => c.ID == userId);
        }

        public void Select(IElasticClient client)
        {
            int userId = 10001;
            //������ѯ
            var user = client.FirstOrDefault<UserESModel>(t => t.UserName.Contains("ceshi") && t.ID == userId && t.Money != 0 && t.CreateAt < DateTime.Now);
            //�Ƿ����
            bool exists = client.Any<UserESModel>(t => t.ID == userId);
            //��¼��
            int count = client.Count<UserESModel>(t => t.ID == userId);
            //���ֵ
            decimal max = client.Max<UserESModel, decimal>(c => c.Money);
            //��Сֵ
            decimal min = client.Min<UserESModel, decimal>(c => c.Money, t => t.Money > 0);
            //ƽ��ֵ
            decimal average = client.Average<UserESModel, decimal>(c => c.Money);
        }



    }
}
