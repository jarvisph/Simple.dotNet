using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Languages
{
    /// <summary>
    /// ISO-639-1 语言代码
    /// </summary>
    public sealed class ISO6391Attribute : Attribute
    {
        public ISO6391Attribute(string code)
        {
            this.Code = code;
        }

        /// <summary>
        /// ISO-639-1 语言代码
        /// </summary>
        public string Code { get; private set; }
        public static implicit operator string(ISO6391Attribute IOS6391)
        {
            return IOS6391.Code;
        }
    }
}
