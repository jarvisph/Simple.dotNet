using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Encryption
{
    public class PwdEncryption
    {
        public static string Encryption(string password)
        {
            return MD5Encryption.Encryption(password);
        }
        public static string Encryption(string password, int id)
        {
            return Encryption(password, id.ToString());
        }
        public static string Encryption(string password, long key)
        {
            return Encryption(password, key.ToString());
        }
        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password">明文</param>
        /// <param name="secretKey">密文</param>
        /// <returns></returns>
        public static string Encryption(string password, string secretKey)
        {
            return MD5Encryption.Encryption(password + secretKey).ToLower();
        }
        /// <summary>
        /// 随机密码
        /// </summary>
        /// <returns></returns>
        public static string RandomPassword(int count = 8)
        {
            return MD5Encryption.MD5Salt(count);
        }
    }
}
