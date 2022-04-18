using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<object> ToArray(this Array array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return array.GetValue(i);
            }
        }
        public static IEnumerable<T> ToArray<T>(this Array array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return (T)array.GetValue(i);
            }
        }
    }
}
