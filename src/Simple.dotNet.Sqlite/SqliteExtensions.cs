using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Sqlite
{
    /// <summary>
    /// sqlite扩展类
    /// </summary>
    public static class SqliteExtensions
    {
        /// <summary>
        ///  IDataReader赋值实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="TEntity"></param>
        /// <returns></returns>
        public static TEntity GetReaderData<TEntity>(this IDataReader reader) where TEntity : IEntity
        {
            TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
            //映射数据库中的字段到实体属性
            IEnumerable<ColumnProperty> propertys = typeof(TEntity).GetColumns();
            foreach (ColumnProperty property in propertys)
            {
                //对实体属性进行设值
                object value = reader[property.Name];
                object obj;
                if (value == null) continue;
                // sqlite 仅 四种数据类型，此处需要将四种数据类型进行强制转换
                // NULL 值是一个 NULL 值。
                // INTEGER 值是一个带符号的整数，根据值的大小存储在 1、2、3、4、6 或 8 字节中。
                // REAL 值是一个浮点值，存储为 8 字节的 IEEE 浮点数字。
                // TEXT 值是一个文本字符串，使用数据库编码（UTF - 8、UTF - 16BE 或 UTF - 16LE）存储。
                // BLOB 值是一个 blob 数据，完全根据它的输入存储。
                switch (property.Property.PropertyType.Name)
                {
                    case "Int16":
                        obj = Convert.ToInt16(value);
                        break;
                    case "Int32":
                        obj = Convert.ToInt32(value);
                        break;
                    case "Int64":
                        obj = Convert.ToInt64(value);
                        break;
                    case "Double":
                        obj = Convert.ToDouble(value);
                        break;
                    case "Decimal":
                        obj = Convert.ToDecimal(value);
                        break;
                    case "Boolean":
                        obj = (long)value == 1 ? true : false;
                        break;
                    case "DateTime":
                        obj = Convert.ToDateTime(value);
                        break;
                    default:
                        if (property.Property.PropertyType.IsEnum)
                        {
                            obj = Enum.GetUnderlyingType(property.Property.PropertyType).Name switch
                            {
                                "Int32" => Convert.ToInt32(value),
                                "Int16" => Convert.ToInt16(value),
                                "Byte" => Convert.ToByte(value),
                                _ => value,
                            };
                        }
                        else
                        {
                            obj = value;
                        }
                        break;
                }
                property.Property.SetValue(entity, obj);
            }
            return entity;
        }
    }
}
