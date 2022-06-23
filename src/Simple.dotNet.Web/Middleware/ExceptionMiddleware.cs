using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Simple.Core.Authorization;
using Simple.Core.Dapper;
using Simple.Core.Domain.Dto;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using Simple.Core.Helper;
using Simple.Core.Logger;
using Simple.Core.Dependency;

namespace Simple.Web.Middleware
{
    /// <summary>
    /// 错误异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public ExceptionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            this._configuration = configuration;
            this._logger = IocCollection.Resolve<ILogger>();
        }
        public virtual async Task Invoke(HttpContext context)
        {
            try
            {
                StopwatchHelper.StartTime();
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
            finally
            {
                StopwatchHelper.StopTime();
            }

        }
        /// <summary>
        /// 发生异常事件处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = ContentType.JSON.GetDescription();
            context.Response.StatusCode = 200;
            Console.WriteLine(exception);
            if (exception is MessageException)
            {
                return context.Response.WriteAsync(new Result(false, exception.Message).ToString());
            }
            else if (exception is AuthorizationException)
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
            else if (exception is DapperException)
            {
                context.Response.StatusCode = 500;
                return context.Response.WriteAsync(new Result(false, Guid.NewGuid().ToString("N")).ToString());
            }
            else
            {
                _logger?.Log(exception);
                return context.Response.WriteAsync(new Result(false, Guid.NewGuid().ToString("N")).ToString());
            }
        }
    }
}
