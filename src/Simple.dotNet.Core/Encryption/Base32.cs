using System.Text;

namespace Simple.Core.Encryption
{
    public static class Base32
    {
        private static readonly char[] Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        public static string ToBase32(this string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            StringBuilder base32String = new StringBuilder();
            int bitIndex = 0;
            int currentByte = 0;
            int byteIndex = 0;

            while (byteIndex < inputBytes.Length)
            {
                currentByte = (currentByte << 8) | inputBytes[byteIndex++];
                bitIndex += 8;

                while (bitIndex >= 5)
                {
                    base32String.Append(Base32Chars[(currentByte >> (bitIndex - 5)) & 0x1F]);
                    bitIndex -= 5;
                }
            }

            if (bitIndex > 0)
            {
                base32String.Append(Base32Chars[(currentByte << (5 - bitIndex)) & 0x1F]);
            }

            return base32String.ToString();
        }
    }
}
