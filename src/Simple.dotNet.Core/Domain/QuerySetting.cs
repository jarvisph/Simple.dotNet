using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Simple.Core.Extensions;

namespace Simple.Core.Domain
{
    public abstract class QuerySetting
    {
        public QuerySetting()
        {

        }
        public QuerySetting(string queryString)
        {
            NameValueCollection request = HttpUtility.ParseQueryString(queryString ?? string.Empty);
            foreach (PropertyInfo property in this.GetType().GetProperties().Where(c => c.CanWrite))
            {
                string name = string.Empty;
                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();
                if (column == null)
                {
                    name = property.Name;
                }
                else
                {
                    name = column.Name;
                }
                if (request.AllKeys.Contains(name))
                {
                    object value = request[name];
                    switch (property.PropertyType.Name)
                    {
                        case "Boolean":
                            value = value.Equals("1") || value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase);
                            break;
                        case "Int32[]":
                            value = ((string)value).ToEnumerable<int>().ToArray();
                            break;
                        case "String[]":
                            value = ((string)value).ToEnumerable<string>().ToArray();
                            break;
                        case "Byte[]":
                            value = ((string)value).ToEnumerable<byte>().ToArray();
                            break;
                        case "Guid":
                            value = ((string)value).ToValue<Guid>();
                            break;
                        default:
                            if (property.PropertyType.IsArray)
                            {
                                Type arrayType = property.PropertyType.GetElementType();
                                string[] values = ((string)value).Split(",");
                                Array array = Array.CreateInstance(arrayType, values.Length);
                                for (int i = 0; i < array.Length; i++)
                                {
                                    array.SetValue(values[i], i);
                                }
                                value = array;
                            }
                            else if (property.PropertyType.IsEnum)
                            {

                            }
                            break;
                    }
                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType), null);
                }
            }
        }
        public override string ToString()
        {
            List<string> list = new List<string>();
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                object value = property.GetValue(this, null);
                if (value != null)
                {
                    switch (property.PropertyType.Name)
                    {
                        case "Int32[]":
                            value = string.Join(",", (int[])value);
                            break;
                        case "String[]":
                            value = string.Join(",", (string[])value);
                            break;
                        default:
                            if (property.PropertyType.IsArray)
                            {
                                Array array = (Array)value;
                                string[] arrayValue = new string[array.Length];
                                for (int i = 0; i < array.Length; i++)
                                {
                                    arrayValue[i] = array.GetValue(i).ToString();
                                }
                                value = string.Join(",", arrayValue);
                            }
                            break;
                    }
                }
                list.Add(string.Format("{0}={1}", property.Name, value == null ? "" : HttpUtility.UrlEncode(value.ToString())));
            }
            return string.Join("&", list);
        }
    }
}
