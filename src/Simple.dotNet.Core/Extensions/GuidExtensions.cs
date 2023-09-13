using System;

namespace Simple.Core.Extensions
{
    public static class GuidExtensions
    {
        public static long ToNumber(this Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

    }
}
