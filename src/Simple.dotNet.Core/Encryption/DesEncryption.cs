using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Simple.dotNet.Core.Encryption
{
    public class DesEncryption
    {
        private static byte[] _key = Encoding.UTF8.GetBytes("Simple");
        public static string Encrytion(string value)
        {
            using (DES des = DES.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                var result = des.CreateDecryptor(_key, data);
                return result.ToString();
            }
        }
    }
}
