using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.Core.Encryption
{
    public static class CryptoNet
    {
        public static string RandomBytesToHex(int byteLength)
        {
            // 1. 使用密码学安全的随机数生成器 (CSPRNG) 生成随机字节
            byte[] randomBytes = RandomNumberGenerator.GetBytes(byteLength);

            // 2. 将字节数组转换为十六进制字符串
            // 使用 Convert.ToHexString，在 .NET 5+ 中可用且性能极佳
            return Convert.ToHexString(randomBytes).ToLowerInvariant();
        }
        public class enc
        {
            public class Utf8
            {
                public static byte[] Parse(string key)
                {
                    return Encoding.UTF8.GetBytes(key);
                    //byte[] keyBytes = new byte[8];
                    //Array.Copy(secretBytes, keyBytes, Math.Min(secretBytes.Length, keyBytes.Length));
                    //return keyBytes;
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
            // 如果需要 Base64 格式
            public static string SHA1Base64(string message, string key)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message ?? "");
                byte[] keyBytes = Encoding.UTF8.GetBytes(key ?? "");

                using (HMACSHA1 hmac = new HMACSHA1(keyBytes))
                {
                    byte[] hashBytes = hmac.ComputeHash(messageBytes);
                    return Convert.ToBase64String(hashBytes);
                }
            }
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
            /// <returns></returns>
            public static string Encrypt(string plainText, byte[] key, CryptoOptions options)
            {
                byte[] encrypted;
                using (RijndaelManaged rmd = new RijndaelManaged())
                {
                    rmd.Key = key;
                    if (options.IV != null)
                    {
                        rmd.IV = options.IV;
                    }
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

            public static string EncryptOFB(string plainText, string key, string iv)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.ECB; // OFB 模式下使用 ECB 作为底层
                    aes.Padding = PaddingMode.None;

                    // 手动实现 OFB 模式
                    int blockSize = aes.BlockSize / 8; // 16 bytes for AES
                    byte[] output = new byte[plainBytes.Length];

                    // 创建加密器
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] feedback = new byte[blockSize];
                        Array.Copy(ivBytes, feedback, Math.Min(blockSize, ivBytes.Length));

                        // 处理每个块
                        for (int i = 0; i < plainBytes.Length; i += blockSize)
                        {
                            // 加密反馈寄存器
                            byte[] encryptedFeedback = encryptor.TransformFinalBlock(feedback, 0, blockSize);

                            // 明文与加密反馈异或
                            int length = Math.Min(blockSize, plainBytes.Length - i);
                            for (int j = 0; j < length; j++)
                            {
                                output[i + j] = (byte)(plainBytes[i + j] ^ encryptedFeedback[j]);
                            }

                            // 更新反馈寄存器（OFB 模式使用加密后的反馈）
                            Array.Copy(encryptedFeedback, feedback, blockSize);
                        }
                    }

                    return BitConverter.ToString(output).Replace("-", "").ToLower();
                }
            }

            public static string DecryptOFB(byte[] ciphertext, string key, string iv)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 128;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.ECB; // OFB模式基于ECB实现
                    aes.Padding = PaddingMode.None;

                    using (var encryptor = aes.CreateEncryptor(keyBytes, new byte[16])) // 使用空IV
                    {
                        // OFB模式：通过对IV加密生成密钥流
                        byte[] keystream = new byte[16];
                        byte[] output = new byte[ciphertext.Length];

                        // 初始化反馈为IV
                        byte[] feedback = new byte[16];
                        Array.Copy(ivBytes, feedback, iv.Length);

                        int processed = 0;

                        while (processed < ciphertext.Length)
                        {
                            // 加密反馈寄存器
                            encryptor.TransformBlock(feedback, 0, feedback.Length, keystream, 0);

                            // 更新反馈为上一个密钥流（OFB特性）
                            Array.Copy(keystream, feedback, feedback.Length);

                            // 使用密钥流解密当前块
                            for (int i = 0; i < 16 && processed + i < ciphertext.Length; i++)
                            {
                                output[processed + i] = (byte)(ciphertext[processed + i] ^ keystream[i]);
                            }

                            processed += 16;
                        }

                        // 移除可能的填充（如果需要）
                        return Encoding.UTF8.GetString(output).TrimEnd('\0');
                    }
                }
            }
        }

        public class SHA256
        {
            public static byte[] Encrypt(string plainText)
            {
                using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] data = Encoding.UTF8.GetBytes(plainText);
                    return sha.ComputeHash(data);
                }
            }
        }

        public class RSA
        {
            public static string CleanPemKey(string pemKey)
            {
                // 移除可能的BOM和空白字符
                pemKey = pemKey.Trim().Replace("\r\n", "\n");

                // 确保有正确的PEM头尾
                if (!pemKey.Contains("-----BEGIN PUBLIC KEY-----"))
                {
                    // 尝试从Base64字符串重建
                    string base64 = Regex.Replace(pemKey, @"-+[A-Z ]+-+|\s+", "");

                    var sb = new StringBuilder();
                    sb.AppendLine("-----BEGIN PUBLIC KEY-----");

                    // 每64字符换行
                    for (int i = 0; i < base64.Length; i += 64)
                    {
                        int len = Math.Min(64, base64.Length - i);
                        sb.AppendLine(base64.Substring(i, len));
                    }

                    sb.AppendLine("-----END PUBLIC KEY-----");
                    return sb.ToString();
                }

                return pemKey;
            }

            public static byte[] GetBytesFromPem(string pem, string section)
            {
                string header = $"-----BEGIN {section}-----";
                string footer = $"-----END {section}-----";

                int start = pem.IndexOf(header) + header.Length;
                int end = pem.IndexOf(footer, start);

                string base64 = pem[start..end].Replace("\n", "").Replace("\r", "");

                return Convert.FromBase64String(base64);
            }
            public static string Encrypt(string plainText, string publicKey)
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // 导入公钥
                    rsa.ImportFromPem(publicKey.ToCharArray());
                    byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);
                    // 使用公钥加密数据
                    byte[] encryptedData = rsa.Encrypt(dataBytes, false);
                    return Convert.ToBase64String(encryptedData);
                }
            }
            public static string Encrypt(byte[] plainText, byte[] publicKey)
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // 导入公钥
                    rsa.ImportPkcs8PrivateKey(publicKey, out _);
                    byte[] encrypted = rsa.Encrypt(plainText, RSAEncryptionPadding.Pkcs1);
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
    }
}
