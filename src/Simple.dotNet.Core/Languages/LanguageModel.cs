using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.Core.Languages
{
    /// <summary>
    /// 多语种模型
    /// </summary>
    public class LanguageModel : Dictionary<LanguageType, string>
    {
        public LanguageModel(string jsonStr)
        {
            this.Clear();
            Dictionary<LanguageType, string> translate = null;
            try
            {
                translate = string.IsNullOrWhiteSpace(jsonStr) ? new Dictionary<LanguageType, string>() : JsonConvert.DeserializeObject<Dictionary<LanguageType, string>>(jsonStr);
            }
            catch
            {
                translate = new Dictionary<LanguageType, string>();
            }

            foreach (KeyValuePair<LanguageType, string> item in translate)
            {
                if (!this.ContainsKey(item.Key))
                {
                    this.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 不存在语种则为英文
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string Get(LanguageType language)
        {
            return this.GetValueOrDefault(language, this.ContainsKey(LanguageType.ENG) ? this[LanguageType.ENG] : string.Empty);
        }
        /// <summary>
        /// 设置语种
        /// </summary>
        /// <param name="language"></param>
        /// <param name="value"></param>
        public void Set(LanguageType language, string value)
        {
            if (this.ContainsKey(language))
            {
                this[language] = value;
            }
            else
            {
                this.Add(language, value);
            }
        }
        /// <summary>
        /// 默认获取中文语种
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return this.GetValueOrDefault(LanguageType.CHN);
        }
        public static implicit operator string(LanguageModel language)
        {
            return language.ToJson();
        }

        public static implicit operator LanguageModel(string jsonStr)
        {
            return new LanguageModel(jsonStr);
        }
        public override string ToString()
        {
            return this.ToJson();
        }

        public static implicit operator bool(LanguageModel language)
        {
            return language.Count != 0;
        }
    }
}
