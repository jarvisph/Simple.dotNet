using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Simple.Core.Extensions;

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
                    obj = (decimal)value;
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
        private static object ToEnumValue(this Type type, long value)
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
        /// 转换RedisValue类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static RedisValue ToRedisValue(this object obj)
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
                    value = (double)obj;
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
        /// redis哈希数组转结构体（反射赋值性能有所耗损）
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static TStruct GetReidsHashValue<TStruct>(this HashEntry[] hash) where TStruct : struct
        {
            object itme = Activator.CreateInstance<TStruct>();
            Dictionary<string, RedisValue> values = new Dictionary<string, RedisValue>();
            foreach (HashEntry item in hash)
            {
                values.Add(item.Name, item.Value);
            }
            if (values.Count == 0) return (TStruct)itme;

            FieldInfo[] fields = typeof(TStruct).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (values.ContainsKey(field.Name))
                {
                    object value = values[field.Name].GetRedisValue(field.FieldType);
                    field.SetValue(itme, value);
                }
                else
                {
                    field.SetValue(itme, field.FieldType.GetDefaultValue());
                }

            }
            return (TStruct)itme;
        }
        private static Dictionary<Type, FieldInfo[]> _fieldCache = new Dictionary<Type, FieldInfo[]>();
        private static FieldInfo[] GetFields(Type type)
        {
            if (!_fieldCache.ContainsKey(type))
            {
                lock (_fieldCache)
                {
                    _fieldCache.Add(type, type.GetFields());
                }
            }
            return _fieldCache[type];
        }
        /// <summary>
        /// 结构体转redis哈希数组（反射赋值性能有所耗损）
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HashEntry[] GetRedisHashEntry<TStruct>(this TStruct value) where TStruct : struct
        {
            List<HashEntry> span = new List<HashEntry>();
            FieldInfo[] fields = GetFields(typeof(TStruct));
            foreach (FieldInfo field in fields)
            {
                span.Add(new HashEntry(field.Name, field.GetValue(value).ToRedisValue()));
            }
            return span.ToArray();
        }
    }
}
