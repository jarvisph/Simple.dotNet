using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Simple.dotNet.Core.Encryption
{
    public class MD5Encryption
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
