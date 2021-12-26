using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;

namespace Simple.dotNet.Core.Dapper
{
    public class DapperException : Exception
    {
        public override string Message { get; }
        /// <summary>
        /// 执行语句
        /// </summary>
        public string CmdText { get; set; }
        /// <summary>
        /// 参数Json
        /// </summary>
        public object Param { get; set; }
        public DapperException(string message)
        {
            this.Message = message;
        }
        public DapperException(string cmdText, string message)
        {
            this.CmdText = cmdText;
            this.Message = message;
        }
        public DapperException(string cmdText, string message, object param)
        {
            this.CmdText = cmdText;
            this.Message = message;
            this.Param = param;
        }
        public override string ToString()
        {
            return this.Message + "\n" + this.CmdText + "\n" + this.Param.ToJson();
        }
    }
}
