using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Simple.dotNet.Core.Encryption
{
    public class SHA256Encryption
    {
        public static string Encryption(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                byte[] hash = sha.ComputeHash(data);
                StringBuilder sb = new StringBuilder();
                foreach (var item in hash)
                {
                    sb.Append(item.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static long GetLongHashCode(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                byte[] hash = sha.ComputeHash(data);
                long start = BitConverter.ToInt64(hash, 0);
                long medium = BitConverter.ToInt64(hash, 8);
                long end = BitConverter.ToInt64(hash, 24);
                return start ^ medium ^ end;
            }

        }
    }
}
