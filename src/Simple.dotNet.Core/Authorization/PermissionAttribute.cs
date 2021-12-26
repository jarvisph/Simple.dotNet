using System;

namespace Simple.dotNet.Core.Authorization
{
    /// <summary>
    /// 功能权限特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : Attribute
    {
        public string Permission { get; set; }

        public PermissionAttribute(string premission)
        {
            this.Permission = premission;
        }
    }
}
