using System;
using Microsoft.Extensions.Caching.Memory;

namespace Simple.dotNet.Core.Helper
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class MemoryHelper
    {
        public static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 获取并设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GetOrCreate<T>(string key, TimeSpan? time, Func<T> action)
        {
            return _memoryCache.GetOrCreate(key, factory =>
             {
                 if (time.HasValue)
                     factory.SetSlidingExpiration(time.Value);
                 return action();
             });
        }
        public static T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        public static T Get<T>(string key, Func<T> action)
        {
            return GetOrCreate(key, null, action);
        }
        /// <summary>
        /// 清除键
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Set<T>(string key, TimeSpan time, T value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(time);
            return _memoryCache.Set(key, value, cacheEntryOptions);
        }
        /// <summary>
        /// 设置键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static T Set<T>(string key, T value)
        {
            return _memoryCache.Set(key, value);
        }
    }
}
