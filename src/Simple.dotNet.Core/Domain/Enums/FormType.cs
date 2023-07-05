using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Domain.Enums
{
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum FormType : byte
    {
        Text = 1,
        Password = 2,
        Radio = 3,
        Checkbox = 4,
    }
}
