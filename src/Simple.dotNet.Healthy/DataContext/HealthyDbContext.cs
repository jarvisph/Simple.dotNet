using Microsoft.EntityFrameworkCore;
using Simple.dotNet.Healthy.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Healthy.DataContext
{
    public class HealthyDbContext : DbContext
    {
        public HealthyDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<HealthExamination> HealthExamination { get; set; }
    }
}
