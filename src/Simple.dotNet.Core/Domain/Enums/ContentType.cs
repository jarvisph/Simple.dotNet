using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Simple.Core.Domain.Enums
{
    /// <summary>
    /// 格式类型
    /// </summary>
    public enum ContentType : byte
    {
        [Description("application/json")]
        JSON,
        [Description("application/x-www-form-urlencoded")]
        Form,
        [Description("application/xml")]
        XML,
        [Description("application/pdf")]
        PDF,
        [Description("application/msword")]
        Word,
        [Description("application/octet-stream")]
        Stream,
        [Description("text/html;charset=UTF-8")]
        Text,
        [Description("image/png")]
        PNG,
        [Description("application/js")]
        JS,
        [Description("application/css")]
        CSS,
        [Description("application/html")]
        HTML,
    }
}
