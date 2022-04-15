using System.Collections.Generic;

namespace Simple.dotNet.Core.Authorization
{
    public interface IPermissionDefinitionContext
    {
        /// <summary>
        /// 创建权限层级（默认Action类型）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="displayName">描述</param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string displayName);
        /// <summary>
        /// 创建权限层级（默认Menu类型）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="displayName">描述</param>
        /// <param name="meta">属性</param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string displayName, PermissionMeta meta);
        /// <summary>
        /// 创建层级关系
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="type"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string displayName, PermissionType type, PermissionMeta meta);
    }
}
