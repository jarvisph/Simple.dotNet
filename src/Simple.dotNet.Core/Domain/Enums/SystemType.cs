using System.ComponentModel;

namespace Simple.Core.Domain.Enums
{
    /// <summary>
    /// 系统类型
    /// </summary>
    public enum SystemType : byte
    {
        /// <summary>
        /// 苹果系统
        /// </summary>
        IOS,
        /// <summary>
        /// 安卓系统
        /// </summary>
        Android,
        /// <summary>
        /// 苹果系统
        /// </summary>
        MacOs,
        /// <summary>
        /// Win系统
        /// </summary>
        Windows,
        /// <summary>
        /// linux系统
        /// </summary>
        Linux,
        /// <summary>
        /// UNIX
        /// </summary>
        Unix,
        /// <summary>
        /// SunOS
        /// </summary>
        SunOS,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }
    /// <summary>
    /// 用户状态枚举
    /// </summary>
    public enum UserStatus : byte
    {
        [Description("正常")]
        Normal = 0,
        [Description("锁定")]
        Lock = 1,
        [Description("删除")]
        Deleted = 10
    }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public enum BrowserType : byte
    {
        [Description("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.5615.87 Safari/537.36")]
        Chrome_Windows_112 = 1,
        [Description("Mozilla/5.0 (Macintosh; Intel Mac OS X 11_2_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.5481.77 Safari/537.36")]
        Chrome_MacOS_110 = 2,
        [Description("Mozilla/5.0 (Linux; Android 11; SM-N770F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.5563.115 Mobile Safari/537.36")]
        Chrome_Andriod_111 = 3,
        [Description("Mozilla/5.0 (iPhone; CPU iPhone OS 15_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/111.0.5563.72 Mobile/15E148 Safari/604.1")]
        Chrome_IOS_111 = 4,


        [Description("Mozilla/5.0 (Windows NT 10.0; Win64; rv:106.0) Gecko/20100101 Firefox/106.0")]
        Firefox_Windows_107,
        [Description("Mozilla/5.0 (Macintosh; Intel Mac OS X 11_12; rv:108.0) Gecko/20100101 Firefox/108.0")]
        Firefox_MacOS_108

    }

    /// <summary>
    /// 代理类型
    /// </summary>
    public enum ProxyType : byte
    {
        [Description("NGINX")]
        NGINX = 1,
        [Description("HTTP")]
        HTTP = 2,
        [Description("HTTPS")]
        HTTPS = 3,
        [Description("SOCKS5")]
        SOCKS5 = 5,
    }

    /// <summary>
    /// 虚拟币钱包类型
    /// </summary>
    public enum WalletType : byte
    {
        [Description("波场")]
        TRC20,

    }

    /// <summary>
    /// 银行卡类型
    /// </summary>
    public enum BankType : int
    {
        [Description("中国银行")]
        BOC = 1,
        [Description("中国农业银行")]
        ABC = 2,
        [Description("中国工商银行")]
        ICBC = 3,
        [Description("中国建设银行")]
        CCB = 4,
        [Description("中国邮政储蓄银行")]
        PSBC = 5,

    }
}
