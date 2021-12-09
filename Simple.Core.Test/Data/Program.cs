using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Data.Repository;
using Simple.Core.Dependency;
using Simple.Core.Localization;
using Simple.Core.Data;
using Simple.Core.Dapper;
using Simple.Test.Model;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using Newtonsoft.Json;
using Simple.Core.Mapper;

namespace Simple.Test.Data
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            var json = JsonConvert.SerializeObject(new { ID = 1 });
            var val = JsonConvert.DeserializeObject<object>(json);
            TestMethod(val);

            var users = new List<User>()
            {
                new User{ ID=1},
                new User{ ID=2},
            };

            var aa = GetList(users, c => new { c.ID }).ToList();

            string connectionString = AppsettingConfig.GetConnectionString("DbConnection");
            IServiceCollection services = new ServiceCollection();
            services.AddDepency();
            services.AddSqlServer(connectionString);
            IReadRepository ReadDB = IocCollection.Resolve<IReadRepository>();
            IWriteRepository WriteDB = IocCollection.Resolve<IWriteRepository>();


            var query = ReadDB.GetAll<User>();
            var user = query.OrderByDescending(c => c.CreateAt).Select(c => new { c.ID }).ToList();
            foreach (var item in user)
            {
            }
            //bool any = query.Any();
            //int count = query.Count();
            foreach (var item in query.OrderByDescending(c => c.CreateAt).Take(100))
            {
                Console.WriteLine($"查询/{item.ID}");
            }
        }
        public IEnumerable<TResult> GetList<TSource, TResult>(List<TSource> list, Func<TSource, TResult> func)
        {
            Func<TSource, TResult> selector = (s) =>
            {
                var value = s.GetType().GetProperty("ID").GetValue(s);
                var result = new { ID = value };
                return default;
            };
            foreach (var item in list)
            {
                yield return selector(item);
            }
        }
        private static void TestMethod(Object x)
        {
            // This is a dummy value, just to get 'a' to be of the right type
            var a = new { ID = 0 };
            a = Cast(a, x);
        }

        private static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }
    }
}
