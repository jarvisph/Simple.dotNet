using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NReJSON;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Redis.Test.NReJson
{
    [TestClass]
    public class Program
    {
        private IConnectionMultiplexer _connectionMultiplexer;
        [TestMethod]
        public void Main()
        {
            string conn = "192.168.0.176:6379";
            _connectionMultiplexer = ConnectionMultiplexer.Connect(conn);
            IDatabase db = _connectionMultiplexer.GetDatabase();
            UserInfo _userInfo = new UserInfo()
            {
                UserID = "AC-67292",
                FullName = "Bob James",
                Email = "bobjames@sample.com",
                Rank = 15,
                Address = new Address()
                {
                    Street = "678 Winona Street",
                    City = "New Medford",
                    State = "Delaware",
                    Zip = "12345"
                }
            };
            var value = new Item { Name = "张三" };
            string key = "userprofile:" + _userInfo.Email.ToLower();
            //db.JsonSet(key, "[]", "Items");
            //db.JsonArrayInsert(key, "Items", 1, JsonConvert.SerializeObject(value));
            //var indexof = db.JsonArrayIndexOf(key, "Items", JsonConvert.SerializeObject(new { Name = "李四" }));
            var result = db.JsonGet(key, "Items.[0]");
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
