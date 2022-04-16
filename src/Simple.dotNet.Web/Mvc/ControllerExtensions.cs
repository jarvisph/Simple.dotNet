using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Simple.Web.Mvc
{
    /// <summary>
    /// controller扩展类
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取controller类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Type GetControllerType(this ActionExecutingContext context)
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).ControllerTypeInfo.AsType();
        }
        /// <summary>
        /// 获取action方法类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static MethodInfo GetActionMethodInfo(this ActionExecutingContext context)
        {
            return ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo;
        }
    }
}
