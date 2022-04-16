using System.Collections.Generic;
using System.Linq;

namespace Simple.Core.Authorization
{
    internal class PermissionDictionary : Dictionary<string, PermissionChildren>
    {
        /// <summary>
        /// 合并所有
        /// </summary>
        public void AddAllPermissions()
        {
            foreach (var permission in Values.ToList())
            {
                AddPermissionRecursively(permission);
            }
        }
        /// <summary>
        /// 递归合并数据
        /// </summary>
        /// <param name="permission"></param>
        private void AddPermissionRecursively(PermissionChildren permission)
        {
            PermissionChildren existingPermission;
            if (TryGetValue(permission.Name, out existingPermission))
            {
                if (existingPermission != permission) throw new AuthorizationException($"{permission.Name},检测到重复的权限名称");
            }
            else
                this[permission.Name] = permission;
            foreach (var childPermission in permission.Children)
                AddPermissionRecursively(childPermission);
        }
    }
}
