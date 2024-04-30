using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NReJSON;
using Simple.Core.Localization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Redis.Test.NReJson
{
    [TestClass]
    public class Program
    {
        private IConnectionMultiplexer _connectionMultiplexer;
        [TestMethod]
        public void Main()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(AppsettingConfig.GetConnectionString("RedisConnection"));
            IDatabase db = _connectionMultiplexer.GetDatabase();
            Dictionary<int, UserInfo> list = new Dictionary<int, UserInfo>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(i, new UserInfo()
                {
                    UserID = i.ToString(),
                    FullName = "Bob James",
                    Email = "bobjames@sample.com",
                    Rank = 15,
                });
            }
            string key = "USERLIST";
            //db.JsonSet(key, JsonConvert.SerializeObject(list));
            //var result = db.JsonGet(key, "1", "2");
            //var s = JsonConvert.DeserializeObject<Dictionary<int, UserInfo>>(result.ToString());
            //db.JsonSet(key, JsonConvert.SerializeObject(888), "1.Rank");

            var result = db.JsonGet(key);
            //db.JsonDelete(key, "0");
            //db.JsonDelete(key);
            //db.JsonSet(key, "[]", "Items");
            //db.JsonArrayInsert(key, "Items", 1, JsonConvert.SerializeObject(value));
            //var indexof = db.JsonArrayIndexOf(key, "Items", JsonConvert.SerializeObject(new { Name = "李四" }));
            //var result = db.JsonGet(key, "Items.[0]");
            //db.JsonSet(key, JsonConvert.SerializeObject(value), "Items");
            //db.JsonSet(key, JsonConvert.SerializeObject("new york"), "Items.1.Name");
            //string json = JsonConvert.SerializeObject(_userInfo);
            //OperationResult result = db.JsonSet(key, JsonConvert.SerializeObject("new york"), ".Address.State");
        }
        public void Set()
        {
            //db.JsonSet(key, JsonConvert.SerializeObject("New York"), ".Address.State");

        }
    }
}
