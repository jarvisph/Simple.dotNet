using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using Simple.Core.Helper;

namespace Simple.Core.Encryption
{
    public static class MD5Encryption
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encryption(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                byte[] hash = md5.ComputeHash(data);
                StringBuilder sb = new StringBuilder();
                foreach (byte item in hash)
                {
                    sb.Append(item.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        private const string MD5CHAR = "0123456789ABCDEF";
        /// <summary>
        /// 返回簡寫的MD5值（64位編碼，4位16進制轉化成爲1位）
        /// </summary>
        /// <param name="md5">必須是大寫的MD5</param>
        /// <returns>8位编码</returns>
        public static string toMD5Short(this string md5)
        {
            md5 = md5.ToUpper();
            md5 = Regex.Replace(md5, $@"[^{MD5CHAR}]", "0");
            int unit = 4;
            if (md5.Length % 4 != 0) md5 = md5.Substring(0, md5.Length / 4 * 4);
            Stack<char> value = new Stack<char>();
            for (int i = 0; i < md5.Length / 4; i++)
            {
                string str = md5.Substring(i * unit, unit);
                int num = 0;
                for (int n = 0; n < str.Length; n++)
                {
                    int charIndex = MD5CHAR.IndexOf(str[n]) * (int)Math.Pow(MD5CHAR.Length, str.Length - n - 1);
                    num += charIndex;
                }
                value.Push(MathHelper.HEX_62[num % MathHelper.HEX_62.Length]);
            }
            return string.Join(string.Empty, value);
        }
        /// <summary>
        /// 获取一个二进制流的MD5值（大寫）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string toMD5(byte[] buffer)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(buffer);
                return string.Join(string.Empty, data.Select(t => t.ToString("x2"))).ToUpper();
            }
        }

        /// <summary>
        /// MD5编码(32位大写）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">默认UTF-8</param>
        /// <returns>默认大写</returns>
        public static string toMD5(this string input, Encoding? encoding = null, int length = 32)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            string md5 = toMD5(encoding.GetBytes(input ?? string.Empty));
            if (length == 32) return md5;
            return md5.Substring(0, length);
        }

        /// <summary>
        /// 对文件流进行MD5加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Encryption(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                foreach (byte item in hash)
                {
                    sb.Append(item.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 生成指定位数的MD5盐
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string MD5Salt(int count = 32)
        {
            string result = string.Empty;
            string[] zm = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                            "5", "6", "7", "8", "9",
                            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                            "0", "1", "2", "3", "4" };
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < count; i++)
            {
                int x = rd.Next(62);
                newRandom.Append(zm[x]);
            }
            result = newRandom.ToString();
            return result;
        }
    }
}
