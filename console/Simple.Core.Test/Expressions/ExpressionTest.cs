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
            int id = 1;
            int[] ints = { 1, 2, 3 };
            Query<User>(c => c.Id == id && !ints.Contains(c.Id) && (c.Age > 10 || c.Created < DateTime.Now) && c.IsAdmin == true && (!c.Name.Contains("ceshi") || c.Name.StartsWith("c") || c.Name.EndsWith("i")));
        }
        public void Cell()
        {
            ServiceCollection services = new ServiceCollection();
            IocCollection.AddCollection(services);
            services.AddSqlServerProvider();

            using (IDapperDatabase db = DbConnectionFactory.CreateDatabase(AppsettingConfig.GetConnectionString("DbConnection"), System.Data.IsolationLevel.Unspecified, DatabaseType.SqlServer))
            {
                var query = db.Query<User>();
                //bool any = query.Any(c => c.Created < DateTime.Now);
                //int count = query.Count(c => c.Age > 10);
                //var user = query.OrderByDescending(c => c.Age).FirstOrDefault(c => c.Id == 1);
                //var group = query.GroupBy(c => new { c.Age }).Select(c => new { c.Key.Age, Count = c.Count() }).ToList();
                var list = query.Skip(10).Take(20).ToList();
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
