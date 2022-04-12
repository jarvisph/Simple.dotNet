using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Elasticsearch
{
    public class ElasticSearchException : Exception
    {
        public ElasticSearchException(string message) : base(message)
        {

        }
    }
}
