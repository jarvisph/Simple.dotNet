using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Simple.Elasticsearch;
using System;
using Simple.ElasticSearch.Test.Model;
using System.Linq;
using Elasticsearch.Net;
using System.Collections.Generic;

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
            //DefaultFieldNameInferrer 保持实体字段与ES索引字段一直，ES默认首字母小写
            var settings = new ConnectionSettings(staticConnectionPool).DisableDirectStreaming().DefaultFieldNameInferrer(name => name);
            IElasticClient client = new ElasticClient(settings);

            int userId = 10001;

            #region 基于Queryable查询
            {
                //query仅拼接查询语句，没有进行真实查询
                var query = client.Query<UserESModel>().Where(c => c.ID == userId)
                                                       .Where(c => c.Money != 0)
                                                       .Where(c => c.SiteID > 0)
                                                       .Where(DateTime.Now, c => c.CreateAt < DateTime.Now)
                                                       .Where(c => c.UserName.Contains("ceshi"))
                                                       .Where(null, t => t.UserName == null);
                //是否存在
                bool exists = query.Any();
                //总数
                int count = query.Count();
                //单条数据
                var user = query.FirstOrDefault();
                //最大值
                decimal max = query.Max(t => t.Money);
                //最小值
                decimal min = query.Min(t => t.Money);
                //平均值
                decimal average = query.Average(t => t.Money);
                //分组聚合
                var group = query.GroupBy(c => true).Select(c => new UserESModel
                {
                    Money = c.Sum(t => t.Money),
                }).FirstOrDefault();

                //条件聚合
                group = query.GroupBy(c => new { c.SiteID, c.ID }).Select(c => new UserESModel
                {
                    SiteID = c.Key.SiteID,
                    Money = c.Sum(t => t.Money),
                }).FirstOrDefault();

                //获取所有数据
                var list = query.ToList();

                //分页获取数据 倒序
                var desc = query.OrderByDescending(c => c.CreateAt).Paged(1, 20, out long total);

                //升序
                var asc = query.OrderBy(c => c.CreateAt).Paged(1, 20, out total);
            }
            #endregion

            #region 扩展类操作（增删改查）
            {
                #region Insert
                {
                    UserESModel user = new UserESModel
                    {
                        ID = 10001,
                        CreateAt = DateTime.Now,
                        Money = 10000,
                        UserName = "ceshi01"
                    };
                    //单个插入
                    client.Insert(user);
                    //批量插入
                    client.Insert(new[] { user });
                }
                #endregion

                #region Delete
                {
                    //删除
                    client.Delete<UserESModel>(c => c.ID == userId);
                }
                #endregion


                #region Update
                //修改指定的字段（PS，如果修改多个字段，建议使用Insert，ES机制如果存在，则修改全部字段）
                client.Update(new UserESModel
                {
                    NickName = "李四",
                    Money = 10001
                }, c => c.ID == userId, c => new
                {
                    c.Money,
                    c.NickName
                });
                //修改单个字段
                client.Update<UserESModel, decimal>(c => c.Money, 100, c => c.ID == userId);

                #endregion

                #region Select

                {
                    //条件查询
                    var user = client.FirstOrDefault<UserESModel>(t => t.UserName.Contains("ceshi") && t.ID == userId && t.Money != 0 && t.CreateAt < DateTime.Now);
                    //是否存在
                    bool exists = client.Any<UserESModel>(t => t.ID == userId);
                    //记录数
                    int count = client.Count<UserESModel>(t => t.ID == userId);
                    //最大值
                    decimal max = client.Max<UserESModel, decimal>(c => c.Money);
                    //最小值
                    decimal min = client.Min<UserESModel, decimal>(c => c.Money, t => t.Money > 0);
                    //平均值
                    decimal average = client.Average<UserESModel, decimal>(c => c.Money);
                }

                #endregion
            }
            #endregion
        }
    }
}
