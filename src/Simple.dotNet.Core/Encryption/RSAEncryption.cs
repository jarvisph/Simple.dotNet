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
        public static string Encrypt(string data, string public_key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // 导入公钥
                rsa.ImportFromPem(public_key);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                // 使用公钥加密数据
                byte[] encryptedData = rsa.Encrypt(dataBytes, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
    }
}
