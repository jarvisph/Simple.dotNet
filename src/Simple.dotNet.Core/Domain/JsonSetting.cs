using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Domain
{
    public abstract class JsonSetting
    {
        public JsonSetting()
        {

        }
        public JsonSetting(string jsonString)
        {
         
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
