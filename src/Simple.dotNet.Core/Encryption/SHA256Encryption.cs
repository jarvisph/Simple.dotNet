using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Simple.Core.Encryption
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

        public static string HMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}
