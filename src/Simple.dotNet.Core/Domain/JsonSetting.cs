using Newtonsoft.Json;
using Simple.Core.Domain.Dto;
using System;

namespace Simple.Core.Domain
{
    public abstract class JsonSetting
    {
        public JsonSetting(string jsonString)
        {
            if (!string.IsNullOrEmpty(jsonString) && jsonString.StartsWith("{"))
            {
                try
                {
                    JsonConvert.PopulateObject(jsonString, this);
                }
                catch
                {
                    Console.WriteLine($"錯誤的數據格式:{jsonString}");
                }
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator string(JsonSetting setting)
        {
            return setting.ToString();
        }


    }
}
