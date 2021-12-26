using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Simple.dotNet.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// IEnumerable 字典装字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, object>> data)
        {
            string queryString = QueryString.Create(data.Select(c => new KeyValuePair<string, string>(c.Key, c.Value?.ToString()))).ToUriComponent();
            return queryString.Substring(1, queryString.Length - 1);
        }

        /// <summary>
        /// foreach循环，返回新集合
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<Result> ForEach<Result, T>(this IEnumerable<T> values, Func<T, Result> action)
        {
            foreach (var item in values)
            {
                yield return action(item);
            }
        }

        /// <summary>
        /// 将集合转成字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinAsString(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
        public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join<T>(separator, source);
        }
    }
}
