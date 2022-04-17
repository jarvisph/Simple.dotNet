using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.MongoDB.Test.Model
{
    [MongoDocument("user")]
    public class User
    {
        [BsonId]
        public int ID { get; set; }
    }
}
