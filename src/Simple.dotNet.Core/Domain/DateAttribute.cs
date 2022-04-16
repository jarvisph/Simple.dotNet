using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Domain
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DateAttribute : Attribute
    {
        public string Format { get; set; }
        public DateAttribute(string format)
        {
            this.Format = format;
        }
    }
}
