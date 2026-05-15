using Microsoft.AspNetCore.Mvc;
using Simple.Core.Dependency;
using Simple.Core.Domain.Dto;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using Simple.Core.Languages;
using Simple.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Web.Mvc
{
    /// <summary>
    /// Controller基类,所有外部Controller继承此基类
    /// </summary>
    [ApiController]
    public abstract class SimpleControllerBase : ControllerBase
    {
        protected ILogger Logger { get; }
        /// <summary>
        /// 构造Controller基类
        /// </summary>
        public SimpleControllerBase()
        {
            this.Logger = IocCollection.Resolve<ILogger>();
        }
       
        /// <summary>
        /// 获取当前http上下文的语种
        /// </summary>
        protected LanguageType Language
        {
            get
            {
                return LanguageType.CHN;
            }
        }
       
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, int page, int limit, Func<T, TResult> selector) => PageResult(query, page, limit, selector, null, null);
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, int page, int limit, Func<T, TResult> selector, object? extend = null) => PageResult(query, page, limit, selector, null, extend);
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, int page, int limit, Func<T, TResult> selector, Action<IEnumerable<T>>? action = null, object? extend = null)
        {
            long total = query.LongCount();
            var items = query.PageBy(page, limit);
            action?.Invoke(items);
            //string json = new PagedResult<TResult>(items.AsEnumerable().Select(selector).ToList(), total, extend).ToString();
            return Ok(new
            {
                success = 1,
                msg = "操作成功",
                data = new
                {
                    items = items.AsEnumerable().Select(selector).ToList(),
                    extend,
                    total
                }
            });
        }
        /// <summary>
        /// json返回，返回数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ActionResult JsonResult(object data)
        {
            return this.JsonResult(true, string.Empty, data);
        }
        /// <summary>
        /// json返回，是否成功
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected ActionResult JsonResult(bool success)
        {
            return this.JsonResult(success, string.Empty, null);
        }
        /// <summary>
        /// json返回
        /// </summary>
        /// <param name="success"></param>
        /// <param name="msg"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        protected ActionResult JsonResult(bool success, string msg, object? info)
        {
            return Ok(new
            {
                success = success ? 1 : 0,
                msg,
                data = info
            });
        }
        protected ActionResult JsonResult(bool success, object data) => this.JsonResult(success, string.Empty, data);
        /// <summary>
        /// 自定义返回
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ActionResult Result(ContentType type, object data)
        {
            return Ok(data);
        }
        /// <summary>
        /// 文本返回
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ActionResult TextResult(string message)
        {
            return Ok(new
            {
                success = 1,
                msg = message,
            });
        }
        /// <summary>
        /// 错误返回
        /// </summary>
        /// <param name="message"></param>
        protected ActionResult ErrorResult(string message)
        {
            return Ok(new
            {
                success = 0,
                msg = message,
            });
        }
    }
}
