using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.MongoDB
{
    public class MongoException : Exception
    {
        public MongoException(string message) : base(message)
        {

        }
    }
}
