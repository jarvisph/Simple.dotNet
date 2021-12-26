using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.Core.Domain
{
    public static class WebAgent
    {
        public static T[] GetArray<T>(string str, char split = ',')
        {
            return str.ToArray<T>(split);
        }
    }
}
