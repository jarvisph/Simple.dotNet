using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Simple.Core.Encryption
{
    public static class CryptoNet
    {
        public class enc
        {
            public class Utf8
            {
                public static byte[] Parse(string key)
                {
                    byte[] secretBytes = Encoding.UTF8.GetBytes(key);
                    byte[] keyBytes = new byte[8];
                    Array.Copy(secretBytes, keyBytes, Math.Min(secretBytes.Length, keyBytes.Length));
                    return keyBytes;
                }
            }
        }
        public class DES
        {
            public static string Encrypt(string plainText, byte[] key, CryptoOptions options)
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                using (System.Security.Cryptography.DES des = System.Security.Cryptography.DES.Create())
                {
                    des.Mode = options.Mode;
                    des.Padding = options.Padding;
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(key, null), CryptoStreamMode.Write))
                        {
                            cs.Write(inputBytes, 0, inputBytes.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
        }

        public class HMAC
        {
            public static string Sha256(string plaintext, string salt)
            {
                var enc = Encoding.Default;
                byte[]
                baText2BeHashed = enc.GetBytes(plaintext),
                baSalt = enc.GetBytes(salt);
                HMACSHA256 hasher = new HMACSHA256(baSalt);
                byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
                return Convert.ToBase64String(baHashedText);
            }
            public static string Sha256HexString(string plaintext, string salt)
            {
                var enc = Encoding.Default;
                byte[]
                baText2BeHashed = enc.GetBytes(plaintext),
                baSalt = enc.GetBytes(salt);
                HMACSHA256 hasher = new HMACSHA256(baSalt);
                byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
                return Convert.ToHexString(baHashedText).ToLower();
            }
        }
        public class AES
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
            public static string Encrypt(string plainText, byte[] key, CryptoOptions options)
            {
                byte[] encrypted;
                using (RijndaelManaged rmd = new RijndaelManaged())
                {
                    rmd.Key = key;
                    rmd.IV = options.IV;
                    rmd.Mode = options.Mode;
                    rmd.Padding = options.Padding;
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

            public static string Decrypt(string plainText, byte[] key, CryptoOptions options)
            {
                using (RijndaelManaged rmd = new RijndaelManaged())
                {
                    rmd.Key = key;
                    if (options.IV != null)
                    {
                        rmd.IV = options.IV;
                    }
                    rmd.Mode = options.Mode;
                    rmd.Padding = options.Padding;
                    ICryptoTransform decryptor = rmd.CreateDecryptor(rmd.Key, rmd.IV);
                    using (MemoryStream stram = new MemoryStream(Convert.FromBase64String(plainText)))
                    {
                        using (CryptoStream decrypt = new CryptoStream(stram, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(decrypt))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

    }
}
