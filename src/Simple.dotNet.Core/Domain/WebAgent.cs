using System;
using System.Collections.Generic;
using System.Text;
using Simple.Core.Extensions;

namespace Simple.Core.Domain
{
    public static class WebAgent
    {
        public static T[] GetArray<T>(string str, char split = ',')
        {
            return str.GetArray<T>(split);
        }

        /// <summary>
        /// 获取当前的时间戳（毫秒，GTM+0）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimestamps()
        {
            return GetTimestamps(DateTime.Now);
        }

        /// <summary>
        /// 获取时间戳（毫秒，GTM+0）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimestamps(this DateTime time)
        {
            return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 时间戳转化成为本地时间（毫秒）
        /// </summary>
        /// <param name="timestamp">时间戳（毫秒）</param>
        /// <returns></returns>
        public static DateTime GetTimestamps(long timestamp)
        {
            return new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset).AddMilliseconds(timestamp);
        }
        /// <summary>
        /// 时间戳转化成为指定时区的时间格式（毫秒)
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="offsetTime"></param>
        /// <returns></returns>
        public static DateTime GetTimestamps(long timestamp, TimeSpan offsetTime)
        {
            return new DateTime(1970, 1, 1).Add(offsetTime).AddMilliseconds(timestamp);
        }
        public static long GetTimestamps(DateTime time, TimeSpan offsetTime)
        {
            return (time.Subtract(offsetTime).Ticks - 621355968000000000) / 10000;
        }
        /// <summary>
        /// 获取两个时间差（时间戳）
        /// </summary>
        /// <param name="startAt"></param>
        /// <param name="endAt"></param>
        /// <returns></returns>
        public static long GetTimeDifference(long startAt, long endAt)
        {
            long start = startAt;
            long end = endAt;
            if (startAt < endAt)
            {
                start = endAt;
                end = startAt;
            }
            return start - end;
        }

        /// <summary>
        /// 产生随机整数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int GetRandom(int min = 0, int max = 100)
        {
            return new Random().Next(min, max);
        }

        /// <summary>
        /// 获取一个指定长度内的随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int GetRandom(int length)
        {
            long number = Guid.NewGuid().ToNumber();
            long quotient = (long)Math.Pow(10, length);
            return (int)(number % quotient);
        }
    }
}
