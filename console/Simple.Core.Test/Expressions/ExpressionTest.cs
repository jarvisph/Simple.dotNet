using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dapper;
using Simple.Core.Data;
using Simple.Core.Data.Expressions;
using Simple.Core.Data.Repository;
using Simple.Core.Dependency;
using Simple.Core.Extensions;
using Simple.Core.Localization;
using Simple.Core.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Test.Expressions
{
    public class ExpressionTest
    {
        public void Where()
        {
            User user = new User
            {
                ID = 1
            };
            int id = 1;
            int[] ints = { 1, 2, 3 };
            Query<User>(c => c.ID == user.ID && !ints.Contains(c.ID) && (c.Money > 10 || c.CreateAt < DateTime.Now) && c.IsAdmin == true && (!c.UserName.Contains("ceshi") || c.UserName.StartsWith("c") || c.UserName.EndsWith("i")));
        }
        public void Cell()
        {
            ServiceCollection services = new ServiceCollection();
            IocCollection.AddCollection(services);
            services.AddSqlServerProvider();

            using (IDapperDatabase db = DbConnectionFactory.CreateDatabase(AppsettingConfig.GetConnectionString("DbConnection"), System.Data.IsolationLevel.Unspecified, DatabaseType.SqlServer))
            {
                var query = db.Query<User>();
                bool any = query.Any(c => c.CreateAt < DateTime.Now);
                int count = query.Count(c => c.Money > 0);
                var user = query.OrderByDescending(c => c.CreateAt).FirstOrDefault(c => c.ID == 1);
                var group = query.GroupBy(c => new { c.Status }).Select(c => new { c.Key.Status, Count = c.Count() }).ToList();
                var list = query.OrderByDescending(c => c.ID).Skip(10).Take(20).ToList();
            }
        }
        public void Query<T>(Expression<Func<T, bool>> expression)
        {
            using (ISqlExpressionVisitor visitor = new SqlServerExpressionVisitor(expression))
            {
                string where = visitor.GetCondition(out DynamicParameters parameters);
                Console.WriteLine($"条件语句：{where}");
                Console.WriteLine($"条件值：{string.Join(",", parameters.ParameterNames.Select(c => $"{c}={parameters.Get<object>(c).GetString()}"))}");
            }
        }
    }
}
