using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.dotNet.Core.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Test.Linq
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            Random random = new Random();
            List<User> list = new List<User>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(new User
                {
                    CreateAt = DateTime.Now,
                    ID = i + 1,
                    Money = random.Next(0, 1000000)
                });
            }

            var total = list.GroupBy(c => true).Select(c => new
            {
                Money = c.Sum(t => t.Money)
            }).FirstOrDefault();
            Console.WriteLine(total);
        }
    }
}
