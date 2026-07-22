using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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



        public PermissionChildren CreatePermission(string name, string path)
        {
            return this.CreatePermission(name, path, null, PermissionType.Action, null);
        }

        public PermissionChildren CreatePermission(string name, string path, string component, PermissionMeta meta)
        {
            return this.CreatePermission(name, path, component, PermissionType.Memu, meta);
        }
        public PermissionChildren CreatePermission(string name, string path, string component, PermissionType type, PermissionMeta meta)
        {
            if (Permissions.ContainsKey(name)) throw new AuthorizationException($"{name},检测到重复的权限名称");
            var permssion = new PermissionChildren(name, path, component, type, meta);
            Permissions[permssion.Name] = permssion;
            return permssion;
        }
    }
}
