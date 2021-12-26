using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.MySql.Test
{
    public class MySqlDbContenxt : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(AppsettingConfig.GetConnectionString("DbConection"));
        }
    }
}
