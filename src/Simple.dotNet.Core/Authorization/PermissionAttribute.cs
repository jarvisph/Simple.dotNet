using System;

namespace Simple.Core.Authorization
{
    /// <summary>
    /// 功能权限特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : Attribute
    {
        /// <summary>
        /// 单权限
        /// </summary>
        public string Permission { get; private set; }
        /// <summary>
        /// 多权限
        /// </summary>
        public string[] Permissions { get; private set; }

        public PermissionAttribute(string premission)
        {
            this.Permission = premission;
            this.Permissions = new string[] { premission };
        }
        public PermissionAttribute(params string[] permissions)
        {
            this.Permissions = permissions;
        }
    }
}
