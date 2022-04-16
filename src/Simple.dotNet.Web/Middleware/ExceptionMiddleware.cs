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
        public ExceptionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            this._configuration = configuration;
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
            else//错误异常
            {
                string logger_url = _configuration["Tool:Logger"];
                if (!string.IsNullOrEmpty(logger_url))
                {
                    Uri uri = new Uri(logger_url);
                    if (string.IsNullOrWhiteSpace(uri.Scheme))
                    {
                        return context.Response.WriteAsync(new Result(false, "错误异常", new
                        {
                            Type = "Exception",
                            exception.Message
                        }).ToString());
                    }
                    Regex regex = new Regex(@"Channel=(?<Token>[\w\\_-]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    Dictionary<string, string> stack = new Dictionary<string, string>();
                    stack.Add("StackTrace", exception.StackTrace);
                    stack.Add("Message", exception.Message);
                    stack.Add("Source", exception.Source);
                    Dictionary<string, object> parmas = new Dictionary<string, object>()
                {
                    {"Title",exception.Message.Substring(0,exception.Message.Length>128?127:exception.Message.Length) },
                    {"RequestInfo", context.GetRequestInfo().Replace("&"," ") },
                    {"Stack",stack.ToJson().Replace("&"," ") },
                    { "Level",LoggerLevel.Error }
                };
                    try
                    {
                        string info = NetHelper.Post(uri.Scheme + "://" + uri.Authority + "/logger/error", parmas, new Dictionary<string, string>() { { "Token", regex.Match(uri.Query).Groups["Token"].Value } });
                        return context.Response.WriteAsync(info);
                    }
                    catch (Exception ex)
                    {
                        return context.Response.WriteAsync(new Result(false, "错误异常", new
                        {
                            Type = "Exception",
                            ex.Message
                        }).ToString());
                    }
                }
                else
                {
                    return context.Response.WriteAsync(new Result(false, "Exception").ToString());
                }
            }
        }
    }
}
