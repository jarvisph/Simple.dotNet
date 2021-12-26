using System.ComponentModel;

namespace Simple.dotNet.Core.Domain.Enums
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
}
