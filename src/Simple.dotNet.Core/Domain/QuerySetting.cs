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
                    object value = request[name].GetValue(property.PropertyType);
                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                }
            }
        }
        public override string ToString()
        {
            List<string> list = new List<string>();
            foreach (PropertyInfo property in this.GetType().GetProperties())
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
                object value = property.GetValue(this);
                if (value != null)
                {
                    if (property.PropertyType.IsArray)
                    {
                        Array array = (Array)value;
                        value = string.Join(",", array.ToArray());
                    }
                }
                list.Add(string.Format("{0}={1}", name, value == null ? string.Empty : HttpUtility.UrlEncode(value.ToString())));
            }
            return string.Join("&", list);
        }
    }
}
