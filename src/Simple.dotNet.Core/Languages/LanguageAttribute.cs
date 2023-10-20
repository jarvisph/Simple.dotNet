using System;

namespace Simple.Core.Languages
{
    public class LanguageAttribute : Attribute
    {
        public LanguageAttribute()
        {

        }

        /// <summary>
        /// 英文
        /// </summary>
        public string ENG { get; set; }

        /// <summary>
        /// 繁体
        /// </summary>
        public string THN { get; set; }

        /// <summary>
        /// 简体中文
        /// </summary>
        public string CHN { get; set; }


    }
}
