using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Authorization
{
    internal abstract class PermissionDefinitionContextBase : IPermissionDefinitionContext
    {
        protected readonly PermissionDictionary Permissions;

        protected PermissionDefinitionContextBase()
        {
            Permissions = new PermissionDictionary();
        }



        public PermissionChildren CreatePermission(string name, string displayName)
        {
            return this.CreatePermission(name, displayName, PermissionType.Action, null);
        }

        public PermissionChildren CreatePermission(string name, string displayName, PermissionMeta meta)
        {
            return this.CreatePermission(name, displayName, PermissionType.Memu, meta);
        }
        public PermissionChildren CreatePermission(string name, string displayName, PermissionType type, PermissionMeta meta)
        {
            if (Permissions.ContainsKey(name)) throw new AuthorizationException($"{name},检测到重复的权限名称");
            var permssion = new PermissionChildren(name, displayName, type, meta);
            Permissions[permssion.Name] = permssion;
            return permssion;
        }
    }
}
