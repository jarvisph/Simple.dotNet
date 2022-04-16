using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Simple.Core.Languages
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType : byte
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [Description("简体中文"), ISO6391("zh-CN")]
        CHN = 0,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [Description("繁体中文"), ISO6391("zh-TW")]
        THN = 1,
        /// <summary>
        /// 英文
        /// </summary>
        [Description("英文"), ISO6391("en")]
        ENG = 2,
        [Description("日语"), ISO6391("ja")]
        JP = 3,
        [Description("韩语"), ISO6391("ko")]
        KR = 4,
        [Description("越南语"), ISO6391("vi")]
        VN = 5,
        [Description("泰语"), ISO6391("th")]
        TH = 6,
        [Description("西班牙语"), ISO6391("es")]
        ES = 7,
        [Description("葡萄牙语"), ISO6391("pt")]
        PT = 8,
        [Description("法语"), ISO6391("fr")]
        FR = 9,
        [Description("德语"), ISO6391("de")]
        DE = 10,
        [Description("意大利语"), ISO6391("it")]
        IT = 11,
        [Description("俄语"), ISO6391("ru")]
        RU = 12
    }
}
