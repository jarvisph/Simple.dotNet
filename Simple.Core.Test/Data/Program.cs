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
using Simple.Test.Model;
using Simple.Core.Test.Model;

namespace Simple.Core.Test.Data
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            string connectionString = AppsettingConfig.GetConnectionString("DbConnection");
            IServiceCollection services = new ServiceCollection();
            services.AddDepency();
            services.AddSqlServer(connectionString);
            IReadRepository ReadDB = IocCollection.Resolve<IReadRepository>();
            IWriteRepository WriteDB = IocCollection.Resolve<IWriteRepository>();

            #region =======新增=====
            //for (int i = 0; i < 1000; i++)
            //{
            //    WriteDB.Insert(new User
            //    {
            //        CreateAt = DateTime.Now,
            //        Money = i,
            //        Status = Core.Domain.Enums.UserStatus.Normal,
            //        UserName = Guid.NewGuid().ToString("N").Substring(0, 8)
            //    });
            //}
            #endregion

            var query = ReadDB.Query<User>();
            var user = query.OrderByDescending(c => c.CreateAt).Select(c => new { c.ID }).ToList();
            foreach (var item in user)
            {
                Console.WriteLine(item.ID);
            }
            bool any = query.Any();
            int count = query.Count();
            foreach (var item in query.OrderByDescending(c => c.CreateAt).Take(100))
            {
                Console.WriteLine($"查询/{item.ID}");
            }
        }

    }
}
