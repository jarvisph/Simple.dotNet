using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Simple.dotNet.Core.Encryption
{
    /// <summary>
    /// AES 加密 解密
    /// </summary>
    public class AesEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipher">默认CBC</param>
        /// <param name="pading">默认PKCS7</param>
        /// <returns></returns>
        public static string AesEncrypt(string plainText, string key, string iv, CipherMode cipher = CipherMode.CBC, PaddingMode pading = PaddingMode.PKCS7)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (RijndaelManaged rmd = new RijndaelManaged())
            {
                rmd.Key = Encoding.UTF8.GetBytes(key);
                rmd.IV = Encoding.UTF8.GetBytes(iv);
                rmd.Mode = cipher;
                rmd.Padding = pading;
                ICryptoTransform encryptor = rmd.CreateEncryptor(rmd.Key, rmd.IV);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(encrypt))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = stream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="plainText">密文</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipher">默认CBC</param>
        /// <param name="pading">默认PKCS7</param>
        /// <returns></returns>
        public static string AesDecrypt(string plainText, string key, string iv, CipherMode cipher = CipherMode.CBC, PaddingMode pading = PaddingMode.PKCS7)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] textBytes = Convert.FromBase64String(plainText);
            string plaintext = null;
            using (RijndaelManaged rmd = new RijndaelManaged())
            {
                rmd.Key = Encoding.UTF8.GetBytes(key);
                rmd.IV = Encoding.UTF8.GetBytes(iv);
                rmd.Mode = cipher;
                rmd.Padding = pading;
                ICryptoTransform decryptor = rmd.CreateDecryptor(rmd.Key, rmd.IV);

                using (MemoryStream stram = new MemoryStream(textBytes))
                {
                    using (CryptoStream decrypt = new CryptoStream(stram, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(decrypt))
                        {
                            plaintext = sr.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
