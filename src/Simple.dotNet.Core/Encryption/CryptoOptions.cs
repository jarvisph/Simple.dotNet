using System.Security.Cryptography;

namespace Simple.Core.Encryption
{
    public class CryptoOptions
    {
        public CipherMode Mode { get; set; } = CipherMode.CBC;
        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
        public byte[] IV { get; set; }
    }
}
