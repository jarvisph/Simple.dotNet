using Simple.Core.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Redis
{
    public static class RedisExtentsions
    {
        public static HashEntry[] ToHashEntries(this object value)
        {
            PropertyInfo[] properties = value.GetType().GetProperties();
            return properties.Where(c => c.GetValue(value) != null).Select(property => new HashEntry(property.Name, property.GetValue(value).ToString())).ToArray();
        }
        public static T GetRedisValue<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance<T>();
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(c => c.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                if (property.PropertyType.IsEnum)
                    property.SetValue(obj, Enum.Parse(property.PropertyType, entry.Value.ToString()), null);
                else
                    property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return obj;
        }
        /// <summary>
        /// 获取T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetRedisValue<T>(this RedisValue value)
        {
            Type type = typeof(T);
            return (T)value.GetRedisValue(type);
        }


        public static object GetRedisValue(this RedisValue value, Type type)
        {
            object obj;
            switch (type.Name)
            {
                case "String":
                    obj = (string)value;
                    break;
                case "Int16":
                    obj = (short)value;
                    break;
                case "Int32":
                    obj = (int)value;
                    break;
                case "Int64":
                    obj = (long)value;
                    break;
                case "Decimal":
                    obj = ((long)value).ToRedisValue();
                    break;
                case "DateTime":
                    obj = new DateTime((long)value);
                    break;
                case "Boolean":
                    obj = (bool)value;
                    break;
                case "Double":
                    obj = (double)value;
                    break;
                case "Guid":
                    obj = Guid.Parse(value);
                    break;
                default:
                    if (type.IsEnum)
                    {
                        obj = type.ToEnumValue((long)value);
                    }
                    else
                    {
                        obj = value;
                    }
                    break;
            }
            return obj;
        }
        /// <summary>
        /// 转换枚举继承的值类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object? ToEnumValue(this Type type, long value)
        {
            if (!type.IsEnum) return null;
            object obj = null;
            switch (Enum.GetUnderlyingType(type).Name)
            {
                case "Int64":
                    obj = value;
                    break;
                case "Int32":
                    obj = (int)value;
                    break;
                case "Int16":
                    obj = (short)value;
                    break;
                case "Byte":
                    obj = (byte)value;
                    break;
            }
            return obj;
        }
        /// <summary>
        /// 整形与MONEY型互转（保留四位小数）
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static long ToRedisValue(this decimal money)
        {
            return (long)(money * 10000M);
        }


        /// <summary>
        /// 整形转为MONEY（保留四位小数）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToRedisValue(this long value)
        {
            return (decimal)value / 10000M;
        }

        /// <summary>
        /// 转换RedisValue类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static RedisValue GetRedisValue(this object obj)
        {
            if (obj == null) return new RedisValue();
            Type type = obj.GetType();
            RedisValue value;
            switch (type.Name)
            {
                case "String":
                    value = (string)obj;
                    break;
                case "Int16":
                    value = (short)obj;
                    break;
                case "Int32":
                    value = (int)obj;
                    break;
                case "Int64":
                    value = (long)obj;
                    break;
                case "Double":
                    value = (double)obj;
                    break;
                case "Decimal":
                    value = ((decimal)obj).ToRedisValue();
                    break;
                case "Byte[]":
                    value = new byte[] { (byte)obj };
                    break;
                case "DateTime":
                    value = ((DateTime)obj).Ticks;
                    break;
                case "Boolean":
                    value = (bool)obj;
                    break;
                case "Guid":
                    value = ((Guid)obj).ToString("N");
                    break;
                default:
                    if (type.IsEnum)
                    {
                        value = Enum.GetUnderlyingType(type).Name switch
                        {
                            "Int64" => (long)obj,
                            "Int32" => (int)obj,
                            "Int16" => (short)obj,
                            _ => (int)(byte)obj,
                        };
                    }
                    else
                        value = (string)obj;
                    break;
            }
            return value;
        }

        /// <summary>
        /// 缓存：属性信息（public、实例、可写、非索引器、非静态）
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

        /// <summary>
        /// 获取可写入的公共实例属性
        /// </summary>
        private static PropertyInfo[] GetProperties(Type type) =>
            _propertyCache.GetOrAdd(type, static t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => p.CanWrite                                   // 必须可写，反序列化要 SetValue
                          && p.GetSetMethod() is not null                // 有公共 setter
                          && p.GetIndexParameters().Length == 0          // 排除索引器 this[int]
                          && p.GetGetMethod() is not { IsStatic: true }  // 排除静态属性
                          && p.GetSetMethod() is not { IsStatic: true })
                 .ToArray());

        /// <summary>
        /// 对象（class）转 Redis 哈希数组（反射赋值性能有所耗损）
        /// </summary>
        public static HashEntry[] GetRedisHashEntry<T>(this T value) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            PropertyInfo[] properties = GetProperties(typeof(T));
            var entries = new HashEntry[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                entries[i] = new HashEntry(property.Name, property.GetValue(value)!.GetRedisValue());
            }
            return entries;
        }

        /// <summary>
        /// redis 哈希数组转对象（class，反射赋值性能有所耗损）
        /// </summary>
        public static T GetReidsHashValue<T>(this HashEntry[] hash) where T : class, new()
        {
            if (hash is null) throw new ArgumentNullException(nameof(hash));

            T item = new T();                       // new() 约束保证有公共无参构造
            if (hash.Length == 0) return item;

            // 建字典：索引器覆盖，避免重复 key 抛异常
            var values = new Dictionary<string, RedisValue>(hash.Length);
            foreach (HashEntry entry in hash)
                values[entry.Name] = entry.Value;

            PropertyInfo[] properties = GetProperties(typeof(T));
            foreach (PropertyInfo property in properties)
            {
                object? raw = values.TryGetValue(property.Name, out RedisValue v)
                    ? v.GetRedisValue(property.PropertyType)
                    : property.PropertyType.GetDefaultValue();

                property.SetValue(item, raw);
            }
            return item;
        }
    }
}
