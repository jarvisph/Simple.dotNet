using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Domain.Dto
{
    /// <summary>
    /// 错误消息返回
    /// </summary>
    public class ErrorMessageResult
    {
        private List<string> message = new List<string>();
        public void Add(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg)) this.message.Add(msg);
        }
        public override string ToString()
        {
            return string.Join("\t", this.message);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="message"></param>
        public static implicit operator string(ErrorMessageResult message)
        {
            return message.ToString();
        }
    }
}
