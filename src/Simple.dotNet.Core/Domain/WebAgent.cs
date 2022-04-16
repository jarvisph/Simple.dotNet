using System;
using System.Collections.Generic;
using System.Text;
using Simple.Core.Extensions;

namespace Simple.Core.Domain
{
    public static class WebAgent
    {
        public static T[] GetArray<T>(string str, char split = ',')
        {
            return str.ToArray<T>(split);
        }
    }
}
