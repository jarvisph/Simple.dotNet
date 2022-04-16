using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.RabbitMQ
{
    /// <summary>
    /// rabbit 异常处理
    /// </summary>
    public class RabbitException : Exception
    {
        public RabbitException(string message) : base()
        {

        }
    }
}
