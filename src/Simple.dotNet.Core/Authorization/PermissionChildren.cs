using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Simple.dotNet.Core.Authorization
{
    /// <summary>
    /// 权限层级关系
    /// </summary>
    public class PermissionChildren
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }
        /// <summary>
        /// 自定义属性
        /// </summary>
        public PermissionMeta Meta { get; set; }
        /// <summary>
        /// 子级
        /// </summary>
        public IReadOnlyList<PermissionChildren> Children => _children.ToImmutableList();
        private readonly List<PermissionChildren> _children;
        public PermissionChildren(string name, string displayName) : this(name, displayName, PermissionType.Action, null)
        {

        }
        public PermissionChildren(string name, string displayName, PermissionMeta meta) : this(name, displayName, PermissionType.Memu, meta)
        {

        }
        public PermissionChildren(string name, string displayName, PermissionType type, PermissionMeta meta)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Type = type;
            this.Meta = meta;
            _children = new List<PermissionChildren>();
        }
        /// <summary>
        /// 创建层级关系（默认权限类型：Action）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public PermissionChildren CreateChildPermission(string name, string displayName)
        {
            return this.CreateChildPermission(name, displayName, PermissionType.Action, null);
        }
        /// <summary>
        /// 创建层级关系（默认权限类型：Menu）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        public PermissionChildren CreateChildPermission(string name, string displayName, PermissionMeta meta)
        {
            return this.CreateChildPermission(name, displayName, PermissionType.Memu, meta);
        }
        /// <summary>
        /// 创建层级关系
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="type"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        public PermissionChildren CreateChildPermission(string name, string displayName, PermissionType type, PermissionMeta meta)
        {
            var permission = new PermissionChildren(name, displayName, type, meta);
            _children.Add(permission);
            return permission;
        }
    }
}
