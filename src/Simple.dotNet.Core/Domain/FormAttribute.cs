using Simple.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Domain
{
    public class FormAttribute : Attribute
    {
        public FormAttribute(FormType form)
        {
            this.Type= form;
        }
        public FormType Type { get; set; }
    }
}
