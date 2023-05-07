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
    }
}
