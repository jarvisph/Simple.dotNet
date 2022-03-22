using System;
using System.Collections.Generic;

namespace Simple.dotNet.Core.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this object value, Type type)
        {
            return type.Name switch
            {
                "String" => value ?? string.Empty,
                "DateTime" => ((DateTime)value).Max(),
                _ => value
            };
        }
        /// 获取type默认值，string 默认是empty
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            object value;
            switch (type.Name)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "Double":
                case "Decimal":
                    value = 0;
                    break;
                case "Boolean":
                    value = false;
                    break;
                case "DateTime":
                    value = Convert.ToDateTime("1900-1-1");
                    break;
                default:
                    value = string.Empty;
                    break;
            }
            return value;
        }
        /// <summary>
        /// 获取安全类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetValue(this Type type, object value)
        {
            object defaultValue = null;
            string name = type.Name;
            if (name == "Nullable`1")
            {
                var generic = type.GenericTypeArguments[0];
                name = generic.Name;
            }

            switch (name)
            {
                case "String":
                    defaultValue = (string)value;
                    break;
                case "Int32":
                    defaultValue = Convert.ToInt32(value);
                    break;
                default:
                    break;
            }
            return defaultValue;
        }
        public static object GetValue(this object value, Type type)
        {
            if (value == null) return type.GetDefaultValue();
            return value.ToValue(type);
        }
        internal static Type GetElementType(this Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }
        private static Type FindIEnumerable(this Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
