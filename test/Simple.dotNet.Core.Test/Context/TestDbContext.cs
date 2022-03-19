using Microsoft.EntityFrameworkCore;
using Simple.dotNet.Core.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Test.Context
{
    internal class TestDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=ES3-DEV;uid=uES3;pwd=RY8S*bS$;database=ES3_Merchant;");
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<User> User { get; set; }
    }
}
