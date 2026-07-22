using System.Collections.Generic;

namespace Simple.Core.Authorization
{
    public interface IPermissionDefinitionContext
    {
        /// <summary>
        /// 创建权限层级（默认Action类型）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">描述</param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string path);
        /// <summary>
        /// 创建权限层级（默认Menu类型）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="meta">属性</param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string path, string component, PermissionMeta meta);
        /// <summary>
        /// 创建层级关系
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        PermissionChildren CreatePermission(string name, string path, string component, PermissionType type, PermissionMeta meta);
    }
}
