using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Simple.Core.Encryption
{
    public class RSAEncryption
    {
        public static KeyValuePair<string, string> Encryption(string value)
        {
            using (RSA ras = RSA.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
            }
            return new KeyValuePair<string, string>();
        }
    }
}
