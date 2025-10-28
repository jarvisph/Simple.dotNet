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
            public class Base64
            {
                public static string Stringify(byte[] byteArray)
                {
                    return Convert.ToBase64String(byteArray);
                }
            }
            public class Hex
            {
                public static byte[] Parse(string hexString)
                {
                    // 移除可能存在的连字符或空格
                    hexString = hexString.Replace("-", "").Replace(" ", "");

                    // 确保字符串长度为偶数
                    if (hexString.Length % 2 != 0)
                    {
                        throw new ArgumentException("十六进制字符串长度必须为偶数");
                    }
                    // 将十六进制字符串转换为字节数组
                    byte[] bytes = new byte[hexString.Length / 2];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }

                    return bytes;
                }
            }
        }
        //public class lib
        //{
        //    public class WordArray
        //    {
        //        public static byte[] Create(byte[] bytes)
        //        {
        //            // 计算需要多少个32位字来存储这些字节
        //            int wordCount = (bytes.Length + 3) / 4; // 向上取整

        //            uint[] words = new uint[wordCount];
        //            int sigBytes = bytes.Length;

        //            // 将字节数组转换为字数组（大端序）
        //            for (int i = 0; i < sigBytes; i++)
        //            {
        //                int wordIndex = i / 4;
        //                int bytePos = 24 - (i % 4) * 8;
        //                words[wordIndex] |= (uint)bytes[i] << bytePos;
        //            }


        //            byte[] buffer = new byte[sigBytes];
        //            for (int i = 0; i < sigBytes; i++)
        //            {
        //                int wordIndex = i / 4;
        //                int bytePos = 24 - (i % 4) * 8;
        //                buffer[i] = (byte)(words[wordIndex] >> bytePos);
        //            }
        //            return buffer;

        //        }
        //    }
        //}
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

            public static string Sha384(string plaintext, string salt)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(salt);
                byte[] messageBytes = Encoding.UTF8.GetBytes(plaintext);

                using (var hmac = new HMACSHA384(keyBytes))
                {
                    byte[] hashBytes = hmac.ComputeHash(messageBytes);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
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
            public static string Decrypt(byte[] plainText, byte[] key, CryptoOptions options)
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
                    using (MemoryStream stram = new MemoryStream(plainText))
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
