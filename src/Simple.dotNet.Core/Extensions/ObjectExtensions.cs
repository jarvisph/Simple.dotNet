using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Simple.dotNet.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 装箱
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// 将给定对象转换为不同类型
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Converted object</returns>
        public static T ToValue<T>(this object obj)
        {
            if (typeof(T) == typeof(Guid))
            {
                if (obj == null)
                {
                    obj = Guid.Empty;
                }
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        public static object ToValue(this object obj, Type type)
        {
            return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 判断是否标记特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this Object obj) where T : Attribute
        {
            ICustomAttributeProvider custom = obj is ICustomAttributeProvider ? (ICustomAttributeProvider)obj : (ICustomAttributeProvider)obj.GetType();
            foreach (var t in custom.GetCustomAttributes(false))
            {
                if (t.GetType().Equals(typeof(T))) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取特性类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj)
        {
            if (obj == null) return default(T);
            ICustomAttributeProvider custom = obj is ICustomAttributeProvider ? (ICustomAttributeProvider)obj : (ICustomAttributeProvider)obj.GetType();
            foreach (var item in custom.GetCustomAttributes(true))
            {
                if (item.GetType().Equals(typeof(T))) return (T)item;
            }
            return default(T);
        }
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        public static string ToJson(this object value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }
        public static T ToObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
