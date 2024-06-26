﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Simple.Core.Domain;
using Simple.Core.Domain.Enums;
using Simple.Core.Domain.Model;
using Simple.Core.Helper;
using Simple.Core.Languages;
using Simple.Core.Mapper;

namespace Simple.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// String转换枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value) where T : IComparable, IFormattable, IConvertible
        {
            if (string.IsNullOrWhiteSpace(value) || !typeof(T).IsEnum) return default;

            Type type = typeof(T);

            if (type.HasAttribute<FlagsAttribute>())
            {
                return ToFlagEnum<T>(value.Split(",").Where(c => !string.IsNullOrWhiteSpace(c) && Enum.IsDefined(type, c.Trim())).Select(c => Enum.Parse(type, value)).ToArray());
            }
            return Enum.IsDefined(type, value) ? (T)Enum.Parse(type, value) : default(T);
        }

        private static T ToFlagEnum<T>(object[] enums) where T : IComparable, IFormattable, IConvertible
        {
            T result;
            switch (Enum.GetUnderlyingType(typeof(T)).Name)
            {
                case "Int16":
                    short int16 = 0;
                    foreach (object value in enums) int16 |= (short)value;
                    result = (T)Enum.ToObject(typeof(T), int16);
                    break;
                case "Int32":
                    int int32 = 0;
                    foreach (object value in enums) int32 |= (int)value;
                    result = (T)Enum.ToObject(typeof(T), int32);
                    break;
                case "Int64":
                    long int64 = 0;
                    foreach (object value in enums) int64 |= (long)value;
                    result = (T)Enum.ToObject(typeof(T), int64);
                    break;
                case "Byte":
                    byte bt = 0;
                    foreach (object value in enums) bt |= (byte)value;
                    result = (T)Enum.ToObject(typeof(T), bt);
                    break;
                default:
                    result = default;
                    break;
            }
            return result;
        }
        public static object ToEnum(this string value, Type type)
        {
            MethodInfo mi = typeof(EnumExtensions).GetMethods().FirstOrDefault(t => t.Name == "ToEnum" && t.IsGenericMethod);
            MethodInfo gmi = mi.MakeGenericMethod(type);
            return gmi.Invoke(null, new object[] { value });
        }

        /// <summary>
        /// 转换枚举值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetEnumValue(this Type type, long value)
        {
            if (!type.IsEnum) return null;
            return Enum.GetUnderlyingType(type).Name switch
            {
                "Int16" => (short)value,
                "Int32" => (int)value,
                "Int64" => value,
                "Byte" => (byte)value,
                _ => null
            };
        }
        /// <summary>
        /// 获取程序集中所有枚举信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetEnums(this Assembly assembly)
        {
            var dic = new Dictionary<string, Dictionary<string, string>>();
            foreach (Type type in assembly.GetTypes().Where(c => c.IsEnum))
            {
                string name = type.FullName;
                dic.Add(name, new Dictionary<string, string>());
                foreach (object item in Enum.GetValues(type))
                {
                    dic[name].Add(item.ToString(), ((Enum)item).GetDescription());
                }
            }
            return dic;
        }
        /// <summary>
        /// 枚举的备注信息缓存
        /// </summary>
        private static Dictionary<string, string> _enumDescription = new Dictionary<string, string>();
        /// <summary>
        /// 获取枚举备注信息
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum em)
        {
            string key = string.Format("{0}.{1}", em.GetType().FullName, em);
            lock (_enumDescription)
            {
                if (_enumDescription.ContainsKey(key)) return (string)_enumDescription[key];
                foreach (FieldInfo field in em.GetType().GetFields())
                {
                    string enumKey = string.Format("{0}.{1}", em.GetType().FullName, field.Name);
                    if (field.IsSpecialName) continue;
                    DescriptionAttribute desc = field.GetAttribute<DescriptionAttribute>();
                    string description = desc == null ? field.Name : desc.Description;
                    if (!_enumDescription.ContainsKey(enumKey)) _enumDescription.Add(enumKey, description);
                }
                return _enumDescription.GetOrDefault(key);
            }
        }
        /// <summary>
        /// 获取枚举备注信息（多语种获取）
        /// </summary>
        /// <param name="em"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum em, LanguageType language)
        {
            string key = string.Format("{0}.{1}.{2}", em.GetType().FullName, em, language);
            lock (_enumDescription)
            {
                if (_enumDescription.ContainsKey(key)) return (string)_enumDescription[key];
                foreach (FieldInfo field in em.GetType().GetFields())
                {
                    string enumKey = string.Format("{0}.{1}.{2}", em.GetType().FullName, field.Name, language);
                    if (field.IsSpecialName) continue;
                    DescriptionAttribute desc = field.GetAttribute<DescriptionAttribute>();
                    string description = desc == null ? field.Name : desc.Description;
                    LanguageAttribute attribute = field.GetAttribute<LanguageAttribute>();
                    if (attribute != null)
                    {
                        PropertyInfo property = typeof(LanguageAttribute).GetProperty(language.ToString());
                        if (property != null)
                        {
                            object value = property.GetValue(attribute);
                            if (value != null)
                            {
                                description = value.ToString();
                            }
                        }
                    }
                    if (!_enumDescription.ContainsKey(enumKey)) _enumDescription.Add(enumKey, description);
                }
                return _enumDescription.GetOrDefault(key);
            }
        }
        /// <summary>
        /// 获取枚举特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="em"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum em) where TAttribute : Attribute
        {
            foreach (FieldInfo field in em.GetType().GetFields().Where(c => c.Name == em.ToString()))
            {
                if (field.IsSpecialName) continue;
                return field.GetAttribute<TAttribute>();
            }
            return (TAttribute)default;
        }
        /// <summary>
        /// 获取配置模块
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static IEnumerable<SettingModel> GetSettings(this Enum em, string jsonString = null)
        {
            Assembly assembly = AssemblyHelper.GetAssemblies().FirstOrDefault(c => c.FullName == em.GetType().Assembly.FullName);
            if (assembly == null) yield break;

            Type type = assembly.GetTypes().FirstOrDefault(c => c.Name == em.ToString());
            if (type == null) yield break;
            object data = null;
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                data = Activator.CreateInstance(type);
            }
            else
            {
                data = JsonConvert.DeserializeObject(jsonString, type);
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                FormType form = FormType.Text;
                string @enum = string.Empty;
                if (property.PropertyType.IsEnum)
                {
                    @enum = property.PropertyType.FullName;
                }
                if (property.HasAttribute<FormAttribute>())
                {
                    FormAttribute attribute = property.GetAttribute<FormAttribute>();
                    form = attribute.Type;
                }
                else
                {
                    form = property.PropertyType.IsEnum ? FormType.Radio : property.PropertyType.Name switch
                    {
                        "String" => FormType.Text,
                        "DateTime" => FormType.DateTime,
                        "Enum" => FormType.Radio,
                        _ => FormType.Text
                    };
                }
                object value = null;
                if (data != null)
                {
                    value = property.GetValue(data);
                }
                yield return new SettingModel()
                {
                    Description = property.GetDescription(),
                    Name = property.Name,
                    Type = form,
                    Value = value,
                    Enum = @enum
                };
            }
        }
    }
}
