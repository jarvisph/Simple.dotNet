using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redis.OM;
using Simple.dotNet.Redis.Test.NReJson;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Redis.Test.RedisOM
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
            var provider = new RedisConnectionProvider(_connectionMultiplexer);
            //provider.Connection.CreateIndex(typeof(UserInfo));
            var customers = provider.RedisCollection<UserInfo>();
            // Insert customer
            //customers.Insert(new Customer()
            //{
            //    FirstName = "James",
            //    LastName = "Bond",
            //    Age = 68,
            //    Email = "bondjamesbond@email.com"
            //});
            var result = customers.Where(x => x.UserID == "AC-67292").ToList();
        }
    }

}
