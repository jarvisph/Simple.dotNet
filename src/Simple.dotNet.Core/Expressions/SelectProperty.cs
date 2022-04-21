using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Expressions
{
    public struct SelectProperty
    {
        public SelectProperty(MemberInfo member, string method, Type property)
        {
            this.Member = member;
            this.Method = method;
            this.PropertyInfo = property;
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public MemberInfo Member { get; set; }
        /// <summary>
        /// 执行的方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 字段属性
        /// </summary>
        public Type PropertyInfo { get; set; }
    }
}
