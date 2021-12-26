using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Simple.dotNet.Core.Data.Schema
{
    /// <summary>
    /// 数据库字段信息
    /// </summary>
    public struct ColumnProperty
    {
        public ColumnProperty(PropertyInfo property)
        {
            this.Property = property;
            ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();
            KeyAttribute key = property.GetCustomAttribute<KeyAttribute>();
            DatabaseGeneratedAttribute generated = property.GetCustomAttribute<DatabaseGeneratedAttribute>();

            this.Name = column == null ? property.Name : column.Name;
            this.IsKey = key != null;
            this.Identity = generated == null ? false : generated.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
            this.IsNull = false;
        }
        public PropertyInfo Property;
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey;
        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string Name;
        /// <summary>
        /// 是否自增
        /// </summary>
        public bool Identity;
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNull;
    }
}
