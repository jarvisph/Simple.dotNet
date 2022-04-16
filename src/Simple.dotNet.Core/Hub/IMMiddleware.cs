using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Simple.Core.Domain.Dto;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using Simple.Core.Helper;

namespace Simple.Core.Hub
{
    /// <summary>
    /// IM中间管道
    /// </summary>
    public class IMMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        public IMMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task Invoke(HttpContext context)
        {
            if (Regex.IsMatch(context.Request.Path, "^im|chat"))
            {
                Uri uri = new Uri(_configuration["Tool:IM"]);
                if (string.IsNullOrWhiteSpace(uri.Scheme)) throw new ArgumentNullException("not uri");
                Regex regex = new Regex(@"Channel=(?<Token>[\w\\_-]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Dictionary<string, object> query = new Dictionary<string, object>();
                foreach (string item in context.Request.Form.Keys)
                {
                    query.Add(item, context.Request.Form[item]);
                }

                try
                {
                    string info = NetHelper.Post(uri.Scheme + "://" + uri.Authority + context.Request.Path.Value.Replace("/im", "").Replace("/chat", ""), query, new Dictionary<string, string>() { { "Token", regex.Match(uri.Query).Groups["Token"].Value } });
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = ContentType.JSON.GetDescription();
                    await context.Response.WriteAsync(info);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = ContentType.JSON.GetDescription();
                    await context.Response.WriteAsync(new Result(false, ex.Message).ToString());
                }
                return;
            }
            await _next(context);
        }
    }
}
