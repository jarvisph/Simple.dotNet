using System;

namespace Simple.dotNet.Core.Authorization
{
    /// <summary>
    /// 标记游客可访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GuestAttribute : Attribute
    {

    }
}
