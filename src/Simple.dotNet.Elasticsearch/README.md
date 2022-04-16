# Simple.Elasticsearch Linq扩展类

> 随着互联网业务发展，我们的系统数据量越来越大，数据库读写能力随之增大，以前我们想到的方案一般是通过数据库发布订阅来实现读写分离；经历实战之后，发现数据库的发布订阅方案维护成本高，查询性能差，一旦更新迭代表、字段，都要重新创建发布订阅，非常繁琐；那有没有一种，在我们系统更新迭代的时候，不需要管理表结构，只需要更新程序，查询能力又强的方案呢？ElasticSearch（以下简体：ES）就非常适合这种场景，它的优点，我就不必多说了，相信大家都有所了解，天生就为了查询而生。不知道大家在使用ES的时候，有没有跟我有过一样的烦恼？官方的net类库使用繁琐、代码可读性差、需要学习DSL语法；我在网上也没有找到合适的组件，我就想，可不可以实现一款linq to elasticsearch一样的组件，对于开放人员来讲，无需学习DSL语法，使用linq to sql一样呢？这个想法在我心里埋下伏笔，就开始上手，一开始我写Simple.Elasticsearch的时候，是扩展官方类库（有兴趣的朋友可以看一下这个版本）也达到了代码简洁、使用简单的扩展类，但应用到项目中，发现一点，没有linq那么灵活，我就动手实现一款linq to elasticsearch，目前组件还不够成熟，但也能够满足基础查询，有兴趣的朋友也可以下载源码一起参与完善组件。Simple.Elasticsearch使用非常简单，下面我来介绍一下组件使用。

## 核心类介绍(索引配置、实体约束、扩展类)
* ElasticSearchIndex
* IDocument
* ElasticSearchExtension


#### 一、ElasticSearchIndex
> ElasticSearchIndex特性类用于设置索引配置，ES实体必须设置

	/// <summary>
    /// ElasticSearchIndex特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ElasticSearchIndexAttribute : Attribute
    {
        /// <summary>
        /// 索引名称（注意，ES索引不支持大写）
        /// </summary>
        public string IndexName { get; private set; }
        /// <summary>
        /// 别名（同理索引规则）
        /// </summary>
        public string[] AliasNames { get; set; }
        /// <summary>
        /// 副本数量（默认0）
        /// </summary>
        public int ReplicasCount { get; set; }
        /// <summary>
        /// 分片数量（默认3）
        /// </summary>
        public int ShardsCount { get; set; }
        /// <summary>
        /// 格式(默认按月创建索引，指定none，不使用索引规则)
        /// </summary>
        public string Format { get; set; }

        public ElasticSearchIndexAttribute(string indexname) : this(indexname, new[] { indexname })
        {

        }
        public ElasticSearchIndexAttribute(string indexname, string[] aliasnams, int replicascount = 0, int shardscount = 3, string fomat = "yyyy_MM")
        {
            this.IndexName = indexname;
            this.AliasNames = aliasnams;
            this.ReplicasCount = replicascount;
            this.ShardsCount = shardscount;
            this.Format = fomat;
        }
        /// <summary>
        /// 自定义索引
        /// </summary>
        /// <param name="datetime"></param>
        public void SetIndexTime(DateTime datetime)
        {
            if (this.Format != "none")
            {
                this.IndexName = $"{this.IndexName}_{datetime.ToString(this.Format)}";
            }
        }
    }

### 二、IDocument

> IDocument用于约束实体类，避免实体冲突  
> ElasticsearchType Nest官方配置，用于配置主键等信息
	
	  /// <summary>
    /// 会员表
    /// </summary>
    [ElasticsearchType(IdProperty = "ID"), ElasticSearchIndex("user", AliasNames = new[] { "user" })]
    public class UserESModel : IDocument
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public int SiteID { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Keyword]
        public string UserName { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public Guid LoginIP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
    }

### 三、ElasticSearchExtension
	 var urls = "http://ES:9200".Split(';').Select(http => new Uri(http));
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


> 作者：Jarvis   
> QQ群：1002557829