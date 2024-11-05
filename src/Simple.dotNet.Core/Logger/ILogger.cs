using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;

namespace Simple.Core.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool Log(string message);
        /// <summary>
        /// 指定来源信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool Log(string message, int userId);
        /// <summary>
        /// 错误异常
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        bool Error(Guid guid, Exception exception);
        bool Error(Guid guid, HttpContext context, Exception exception);
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        void Warn(string message);
        /// <summary>
        /// 警告/异常
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        void Warn(string title, string message);

        void Info(int userId, HttpContext context);

    }
}
