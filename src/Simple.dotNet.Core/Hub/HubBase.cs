using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Core.Hub
{
    /// <summary>
    /// 通信基类
    /// </summary>
    public abstract class HubBase
    {

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="channels">频道</param>
        public abstract void Push(string content, params string[] channels);

        /// <summary>
        /// 获取频道的在线人数
        /// </summary>
        /// <param name="channels">频道</param>
        /// <returns></returns>
        public virtual Dictionary<string, int> GetOnlineCount(params string[] channels)
        {
            return channels.ToDictionary(c => c, c => 0);
        }
    }
}
