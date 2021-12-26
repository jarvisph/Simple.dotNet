using System;

namespace Simple.dotNet.Redis
{
    /// <summary>
    /// Redis异常处理中间件
    /// </summary>
    public class RedisException : Exception
    {
        public override string Message { get; }
        public RedisException(string message)
        {
            this.Message = message;
        }
    }
}
