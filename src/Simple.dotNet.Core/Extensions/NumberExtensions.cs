using System;

namespace Simple.Core.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// 保留小数位数（舍位处理，不做四舍五入）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal ToFixed(this decimal value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.ToNegativeInfinity);
            //AwayFromZero  当一个数字是其他两个数字的中间值时，会将其舍入为两个数字中从零开始较接近的数字。
            //ToEven  当一个数字是其他两个数字的中间值时，会将其舍入为最接近的偶数。
            //ToNegativeInfinity  当一个数字是其他两个数字的中间值时，会将其舍入为最接近且不大于无限精确的结果。
            //ToPositiveInfinity 当一个数字是其他两个数字的中间值时，会将其舍入为最接近且不小于无限精确的结果。
            //ToZero  当一个数字是其他两个数字的中间值时，会将其舍入为最接近结果，而不是无限精确的结果。
        }
    }
}
