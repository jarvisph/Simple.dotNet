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
        [Description("简体中文"), ISO6391("zh-CN")]
        CHN = 0,
        [Description("正體中文"), ISO6391("zh-TW")]
        THN = 1,
        [Description("English"), ISO6391("en")]
        ENG = 2,
        [Description("日本語"), ISO6391("ja")]
        JP = 3,
        [Description("한국어"), ISO6391("ko")]
        KR = 4,
        [Description("Tiếng việt"), ISO6391("vi")]
        VN = 5,
        [Description("ไทย"), ISO6391("th")]
        TH = 6,
        [Description("Español"), ISO6391("es")]
        ES = 7,
        [Description("Português"), ISO6391("pt")]
        PT = 8,
        [Description("Français"), ISO6391("fr")]
        FR = 9,
        [Description("Deutsch"), ISO6391("de")]
        DE = 10,
        [Description("Italiano"), ISO6391("it")]
        IT = 11,
        [Description("Русский"), ISO6391("ru")]
        RU = 12,
        [Description("indonesia"), ISO6391("id")]
        ID = 13
    }
}
