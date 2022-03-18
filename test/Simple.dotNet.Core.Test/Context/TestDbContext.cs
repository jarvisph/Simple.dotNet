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
        public DbSet<User> User { get; set; }
    }
}
