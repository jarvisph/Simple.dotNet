using Simple.Core.Domain.Enums;

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
        /// 代理地址
        /// </summary>
        public string Proxy { get; set; }
        /// <summary>
        /// 用户名（需要验证）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        public string GetProxyUrl()
        {
            return Type switch
            {
                ProxyType.NGINX => $"http://{this.Proxy}/?",
                ProxyType.HTTP => $"http://{this.Proxy}",
                ProxyType.SOCKS5 => $"socks5://{this.Proxy}",
                _ => throw new System.Exception($"代理地址获取错误"),
            };
        }
        public string GetProxyFormat()
        {
            return Type switch
            {
                ProxyType.HTTP => $"http://{this.UserName}:{this.Password}@{this.Proxy}",
                ProxyType.SOCKS5 => $"socks5://{this.UserName}:{this.Password}@{this.Proxy}",
                _ => throw new System.Exception($"代理地址获取错误"),
            };
        }

        public bool Check()
        {
            if (this == null) return false;
            if (string.IsNullOrWhiteSpace(Proxy))
            {
                return false;
            }
            return true;
        }
    }
}
