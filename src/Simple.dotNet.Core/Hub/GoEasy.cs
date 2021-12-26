using System.Text;
using System.Web;
using Simple.dotNet.Core.Domain.Enums;
using Simple.dotNet.Core.Helper;

namespace Simple.dotNet.Core.Hub
{
    /// <summary>
    /// GoEasy扩展类
    /// </summary>
    public class GoEasy : HubBase
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// 既可以发送消息或也可以订阅channel来接收消息
        /// </summary>
        public string CommonKey { get; }
        /// <summary>
        /// 只能用来订阅channel来接收消息
        /// </summary>
        public string SubscribeKey { get; set; }

        public GoEasy(string commonkey, string host, string subscribekey)
        {
            this.Host = host;
            this.CommonKey = commonkey;
            this.SubscribeKey = subscribekey;
        }

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="content"></param>
        /// <param name="channels"></param>
        public override void Push(string content, params string[] channels)
        {
            foreach (string channel in channels)
            {
                string postData = $"appkey={CommonKey}&channel={channel}&content={HttpUtility.UrlEncode(content, Encoding.UTF8)}";
                NetHelper.Post($"http://{Host}/publish", ContentType.Form, Encoding.UTF8.GetBytes(postData));
            }
        }
    }
}
