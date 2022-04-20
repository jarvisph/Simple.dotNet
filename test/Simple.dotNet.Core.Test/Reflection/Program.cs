using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Core.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple.dotNet.Core.Test.Reflection
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            User user = new User();
            var type = typeof(User);
            foreach (var item in type.GetProperties().Where(c => c.Name == "UserName"))
            {
                ColumnAttribute column = item.GetAttribute<ColumnAttribute>();
            }
        }
    }
}
