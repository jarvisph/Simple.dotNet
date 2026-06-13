using Simple.Core.Domain.Enums;
using System;

namespace Simple.Core.Domain.Model
{
    /// <summary>
    /// 代理配置
    /// </summary>
    public class ProxySetting
    {
        /// <summary>
        /// 代理类型
        /// </summary>
        public ProxyType Type { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用户名（需要验证）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 延迟
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(10);

        public bool AllowAutoRedirect { get; set; } = true;

        /// <summary>
        /// 代理格式
        /// </summary>
        public string Format => this.GetProxyFormat();

        public string GetProxyUrl()
        {
            return Type switch
            {
                ProxyType.HTTP => $"http://{this.IP}:{this.Port}",
                ProxyType.SOCKS5 => $"socks5://{this.IP}:{this.Port}",
                _ => throw new Exception($"代理地址获取错误：{this.IP}"),
            };
        }
        public void GetProxyUrl(ref string url)
        {
            switch (Type)
            {
                case ProxyType.HTTP:
                    break;
                case ProxyType.HTTPS:
                    break;
                case ProxyType.SOCKS5:
                    break;
                case ProxyType.FF:
                    break;
                default:
                    break;
            }
        }
        public string GetProxyFormat()
        {
            return Type switch
            {
                ProxyType.HTTP => $"http://{this.UserName}:{this.Password}@{this.IP}:{this.Port}",
                ProxyType.SOCKS5 => $"socks5://{this.UserName}:{this.Password}@{this.IP}:{this.Port}",
                ProxyType.FF => this.IP,
                _ => "",
            };
        }

        public bool Check()
        {
            if (this == null) return false;
            if (string.IsNullOrWhiteSpace(this.IP))
            {
                return false;
            }
            return true;
        }
    }
}
