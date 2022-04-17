using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Simple.Core.Extensions;

namespace Simple.Core.Data.Schema
{
    /// <summary>
    /// 数据库结构缓存
    /// </summary>
    public static class SchemaExtensions
    {
        private static readonly Dictionary<Type, string> _TableNameCache = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, List<ColumnProperty>> _ColumnCache = new Dictionary<Type, List<ColumnProperty>>();

        /// <summary>
        /// 获取实体表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            if (_TableNameCache.ContainsKey(type)) return _TableNameCache[type];
            lock (_TableNameCache)
            {
                TableAttribute table = type.GetAttribute<TableAttribute>();
                if (!_TableNameCache.ContainsKey(type)) _TableNameCache.Add(type, table == null ? type.Name : table.Name);
            }
            return _TableNameCache[type];
        }
        /// <summary>
        /// 获取实体类所有字段信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ColumnProperty> GetColumns(this Type type)
        {
            if (_ColumnCache.ContainsKey(type)) return _ColumnCache[type];
            lock (_ColumnCache)
            {
                List<ColumnProperty> columns = new List<ColumnProperty>();
                foreach (PropertyInfo property in type.GetProperties().Where(c => c.PropertyType.IsPublic && !c.HasAttribute<NotMappedAttribute>()))
                {
                    columns.Add(new ColumnProperty(property));
                }
                if (!_ColumnCache.ContainsKey(type)) _ColumnCache.Add(type, columns);
            }
            return _ColumnCache[type];
        }
        /// <summary>
        /// 指定获取实体类中的字段信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IEnumerable<ColumnProperty> GetColumns<TEntity>(this Type type, params Expression<Func<TEntity, object>>[] fields) where TEntity : IEntity
        {
            List<ColumnProperty> list = GetColumns(type);
            if (fields.Length == 0) return list;
            return list.Where(c => fields.Any(t => t.GetPropertyInfo().Name == c.Property.Name));
        }
        /// <summary>
        /// 获取实体中的某个字段值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static ColumnProperty GetColumn<TEntity, TValue>(this Expression<Func<TEntity, TValue>> field) where TEntity : IEntity
        {
            IEnumerable<ColumnProperty> columns = typeof(TEntity).GetColumns();
            PropertyInfo property = field.GetPropertyInfo();
            return columns.FirstOrDefault(c => c.Property.Name == property.Name);
        }
        /// <summary>
        /// 获取数据库主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static ColumnProperty[] GetTableKey(this Type type)
        {
            IEnumerable<ColumnProperty> columns = type.GetColumns();
            return columns.Where(c => c.IsKey).ToArray();
        }
        /// <summary>
        ///  IDataReader赋值实体
        /// </summary>
        /// <returns></returns>
        public static TResult GetReaderData<TResult>(this IDataReader reader)
        {
            return (TResult)reader.GetReaderData(Activator.CreateInstance(typeof(TResult)));
        }
        public static object GetReaderData(this IDataReader reader, object source)
        {
            //映射数据库中的字段到实体属性
            IEnumerable<ColumnProperty> propertys = source.GetType().GetColumns();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                if (!propertys.Any(c => c.Name == fieldName)) continue;
                ColumnProperty property = propertys.FirstOrDefault(c => c.Name == fieldName);
                //对实体属性进行设值
                object value = reader[property.Name];
                if (value == null) continue;
                property.Property.SetValue(source, value);
            }
            return source;
        }
    }
}
