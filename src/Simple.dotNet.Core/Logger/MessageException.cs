using System;

namespace Simple.Core.Logger
{
    /// <summary>
    /// 自定义错误信息
    /// </summary>
    public class MessageException : Exception
    {
        /// <summary>
        /// 信息
        /// </summary>
        public override string Message { get; }
        /// <summary>
        /// 错误编码
        /// </summary>
        public int Code { get; set; }
        public MessageException(string message)
        {
            this.Message = message;
            this.Code = 201;
        }
        public MessageException(string message, int code)
        {
            this.Message = message;
            this.Code = code;
        }
    }
}
