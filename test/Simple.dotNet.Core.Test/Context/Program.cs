﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Test.Context
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            using (TestDbContext db = new TestDbContext())
            {
                var user = db.User.Select(c => new { c.ID }).FirstOrDefault();
            }
        }
    }
}
