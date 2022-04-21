using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Helper
{
    public static class ArrayHelper
    {
        public static T[] CreateInstance<T>(int length, T defaultValue)
        {
            Array array = Array.CreateInstance(typeof(T), length);
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(defaultValue, i);
            }
            return array.ToArray<T>().ToArray();
        }
    }
}
