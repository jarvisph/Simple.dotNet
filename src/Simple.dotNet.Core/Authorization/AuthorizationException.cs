﻿using System;

namespace Simple.Core.Authorization
{
    /// <summary>
    /// 授权异常
    /// </summary>
    public class AuthorizationException : Exception
    {
        /// <summary>
        /// 文本描述
        /// </summary>
        public override string Message { get; }
        public AuthorizationException() { }
        public AuthorizationException(string message)
        {
            this.Message = message;
        }
    }
}
