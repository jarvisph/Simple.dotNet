using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Redis.Test.RedisOM
{
    [Document]
    public class Customer
    {
        [RedisIdField] public Ulid Id { get; set; }
        [Indexed(Sortable = true)] public string FirstName { get; set; }
        [Indexed(Aggregatable = true)] public string LastName { get; set; }
        public string Email { get; set; }
        [Indexed(Sortable = true)] public int Age { get; set; }
    }
}
