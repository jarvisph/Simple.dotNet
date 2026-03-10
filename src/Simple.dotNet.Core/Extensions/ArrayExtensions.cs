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
        public static T Get<T>(this string[] args, string name, T defaultValue)
        {
            int index = Array.IndexOf(args, name);
            if (index == -1 || args.Length <= index + 1) return defaultValue;
            string value = args[index + 1];
            return value.GetValue<T>() ?? defaultValue;
        }
        public static string Get(this string[] args, string name)
        {
            return args.Get(name, string.Empty);
        }

        public static T RandomItem<T>(this IList<T> list)
        {
            int count = list.Count;
            if (count == 0)
            {
                return default(T);
            }

            int num = Math.Abs(Guid.NewGuid().GetHashCode());
            return list.ElementAt(num % count);
        }

        public static T ClosestTo<T>(this IEnumerable<T> source, T target) where T : struct, IComparable<T>, IConvertible
        {
            if (!source.Any())
                throw new InvalidOperationException("序列不包含任何元素");

            return source.Aggregate((current, next) =>
            {
                double currentDiff = Math.Abs(Convert.ToDouble(current) - Convert.ToDouble(target));
                double nextDiff = Math.Abs(Convert.ToDouble(next) - Convert.ToDouble(target));
                return nextDiff < currentDiff ? next : current;
            });
        }
    }
}
