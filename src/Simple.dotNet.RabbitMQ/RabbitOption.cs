using System.ComponentModel.DataAnnotations.Schema;
using Simple.Core.Domain;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// rabbit 连接参数
    /// </summary>
    public class RabbitOption : QuerySetting
    {
        public RabbitOption(string querystring) : base(querystring)
        {

        }
        /// <summary>
        /// 服务地址
        /// </summary>
        [Column("server")]
        public string HostName { get; set; } = string.Empty;
        /// <summary>
        /// 端口
        /// </summary>
        [Column("port")]
        public int Port { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [Column("username")]
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 登录密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 虚拟主机
        /// </summary>
        [Column("virtualhost")]
        public string VirtualHost { get; set; } = string.Empty;

    }
}
