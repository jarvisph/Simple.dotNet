using Dapper;
using Simple.Core.Data.Expressions;
using Simple.Core.Extensions;
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
        public void Any()
        {

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
