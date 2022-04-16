using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Redis.Test.NReJson
{
    public class UserInfo
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Rank { get; set; }
        public Address Address { get; set; }
        public Item[] Items { get; set; }
    }
    public class Item
    {
        public string Name { get; set; }

    }
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
