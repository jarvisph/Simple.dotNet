using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.dotNet.Core.Helper
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public  class RegularHelper
    {
        /// <summary>
        /// 正则表达式替换文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static string Replace(string text, string replacement, string regex)
        {
            RegexOptions ops = RegexOptions.Multiline;
            Regex rg = new Regex(regex, ops);
            if (rg.IsMatch(text))
            {
                return rg.Replace(text, replacement);
            }
            return text;
        }
        public static string Replace(string text, string replacement, Regex regex)
        {
            if (regex.IsMatch(text))
            {
                return regex.Replace(text, replacement);
            }
            return text;
        }
    }
}
