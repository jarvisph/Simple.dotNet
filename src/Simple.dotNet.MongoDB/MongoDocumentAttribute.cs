using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.MongoDB
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoDocumentAttribute : Attribute
    {
        public MongoDocumentAttribute(string collection)
        {
            this.Collection = collection;
        }
        public string Collection { get; }
    }
}
