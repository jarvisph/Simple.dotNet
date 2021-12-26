using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Hub
{
    [Flags]
    public enum IMType : byte
    {
        /// <summary>
        /// 群聊
        /// </summary>
        Group = 1,
        /// <summary>
        /// 私聊
        /// </summary>
        Friend = 2,
        /// <summary>
        /// 群主
        /// </summary>
        Lord = 4,
    }
}
