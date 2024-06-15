using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Simple.Core.Data.Repository;
using Simple.Core.Dependency;
using Simple.Core.Domain.Dto;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using Simple.Core.Languages;
using Simple.Core.Mapper;
using Result = Simple.Core.Domain.Dto.Result;

namespace Simple.Web.Mvc
{
    /// <summary>
    /// Controller基类,所有外部Controller继承此基类
    /// </summary>
    [ApiController]
    public abstract class SimpleControllerBase : ControllerBase
    {
        //protected IWriteRepository WriteRepository { get; }
        //protected IReadRepository ReadRepository { get; }
        ///// <summary>
        ///// 构造Controller基类
        ///// </summary>
        //public SimpleControllerBase()
        //{
        //    this.WriteRepository = IocCollection.Resolve<IWriteRepository>();
        //    this.ReadRepository = IocCollection.Resolve<IReadRepository>();
        //}
        /// <summary>
        /// 页码
        /// </summary>
        protected int PageSize
        {
            get
            {
                return HttpContext.GetFormValue("Page", 1);
            }
        }
        /// <summary>
        /// 行数
        /// </summary>
        protected int LimitCount
        {
            get
            {
                return HttpContext.GetFormValue("Limit", 20);
            }
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
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, Func<T, TResult> selector)
            => this.PageResult(query, selector, null);
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, Func<T, TResult> selector, object extend)
         => this.PageResult(query, selector, null, extend);
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, Func<T, TResult> selector, Action<IEnumerable<T>>? action)
         => this.PageResult(query, selector, action, null);

        /// <summary>
        /// 返回分页格式（带扩展字段）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="selector"></param>
        /// <param name="action"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        protected ActionResult PageResult<T, TResult>(IOrderedQueryable<T> query, Func<T, TResult> selector, Action<IEnumerable<T>>? action, object? extend)
        {
            long total = query.LongCount();
            var items = query.PageBy(this.PageSize, this.LimitCount);
            action?.Invoke(items);
            string json = new PagedResult<TResult>(items.AsEnumerable().Select(selector).ToList(), total, extend).ToString();
            return Ok(json);
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
            return Ok(new Result(success, msg, info).ToString());
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
            return Ok(new Result(true, message).ToString());
        }
        /// <summary>
        /// 错误返回
        /// </summary>
        /// <param name="message"></param>
        protected ActionResult ErrorResult(string message)
        {
            return Ok(new Result(false, message).ToString());
        }
    }
}
