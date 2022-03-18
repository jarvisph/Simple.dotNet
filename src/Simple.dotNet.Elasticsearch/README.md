# Simple.dotNet.Elasticsearch Linq扩展类

> 随着互联网业务发展，我们的系统数据量越来越大，我们数据库读写能力随之增大，以前我们想到的方案一般是通过数据库发布订阅来实现读写分离；经历实战之后，发现数据库的发布订阅方案维护成本高，查询性能差，一旦更新迭代表、字段，都要重新创建发布订阅，非常繁琐；那有没有一种，在我们系统更新迭代的时候，不需要管理表结构，只需要更新程序，查询能力又强的方案呢？下面就是我们要使用的搜索引擎，ElasticSearch（以下简体：ES）它的优点，我就不必多说了，相信大家都有所了解，天生就为了查询而生。不知道大家在使用ES的时候，有没有跟我有过一样的烦恼？官方的net类库使用繁琐、代码可读性差、需要学习DSL语法；我在网上也没有找到合适的组件，我就想，可不可以实现一款类型与linq一样的组件，对于开放人员来讲，无需学习DSL语法，使用linq一样呢？这个想法在我心里埋下伏笔，就开始上手，一开始我写Simple.Elasticsearch的时候，是扩展官方类库（有兴趣的朋友可以看一下这个版本）也达到了代码简洁、使用简单的扩展类，但应用到项目中，发现一点，没有linq那么灵活，我就想干脆实现一款linq to elasticsearch，查阅大量资料，攻克层层问题，终于实现了这款Simple.dotNet.Elasticsearch组件，目前组件还不够成熟，但也能够满足基础查询，有兴趣的朋友也可以下载源码一起参与完善组件。Simple.dotNet.Elasticsearch使用非常简单，下面我来介绍一下组件使用。

* ElasticSearchIndex 特性介绍（核心）
 - IndexName        --索引名称
 - AliasNames        --别名
 - ReplicasCount    --分片数量
 - ShardsCount      --副本数量
 - Format                --索引名格式（默认yyyy_MM）

> ES 实体必须继承IDocument，并标记添加ElasticSearchIndex特性

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


- 后面我们将实现GroupBy聚合等功能，敬请期待吧！！

> 作者：Jarvis 
> QQ群：1002557829