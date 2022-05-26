using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Simple.Core.Extensions;

namespace Simple.Core.Authorization
{
    public class PermissionFinder
    {
        /// <summary>
        /// 不带层级
        /// </summary>
        /// <param name="authorizationProviders"></param>
        /// <returns></returns>
        public static IReadOnlyList<PermissionChildren> GetAllPermission(params AuthorizationProvider[] authorizationProviders)
        {
            return new InternalPermissionFinder(true, authorizationProviders).GetAllPermissions();
        }
        /// <summary>
        /// 带层级
        /// </summary>
        /// <param name="authorizationProviders"></param>
        /// <returns></returns>
        public static IReadOnlyList<PermissionChildren> GetAllPermissionChildren(params AuthorizationProvider[] authorizationProviders)
        {
            return new InternalPermissionFinder(false, authorizationProviders).GetAllPermissions();
        }
        /// <summary>
        /// 获取权限菜单
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public static IReadOnlyList<PermissionChildren> GetMenu(string[] permissions, params AuthorizationProvider[] providers)
        {
            return new InternalPermissionFinder(false, providers).GetMenu(permissions);
        }
        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetPermission(Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                yield return field.GetValue(type).ToString();
            }
        }
        /// <summary>
        /// 获取权限描述
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static string GetDescription(Type type, string permission)
        {
            FieldInfo field = type.GetFields().Where(c => c.Name == permission).FirstOrDefault();
            if (field == null) return string.Empty;
            DescriptionAttribute description = field.GetCustomAttribute<DescriptionAttribute>();
            if (description == null)
            {
                return permission;
            };
            return description.Description;
        }
    }
    internal class InternalPermissionFinder : PermissionDefinitionContextBase
    {
        public InternalPermissionFinder(bool isChildren, params AuthorizationProvider[] authorizationProviders)
        {
            foreach (var provider in authorizationProviders)
                provider.SetPermissions(this);
            //合并所有
            if (isChildren)
                Permissions.AddAllPermissions();
        }
        public IReadOnlyList<PermissionChildren> GetAllPermissions()
        {
            return Permissions.Values.ToImmutableList();
        }

        public IReadOnlyList<PermissionChildren> GetMenu(string[] permissions)
        {
            PermissionDictionary permission = new PermissionDictionary();
            foreach (KeyValuePair<string, PermissionChildren> item in this.Permissions)
            {
                if (permissions.Contains(item.Key) && item.Value.Type == PermissionType.Memu)
                {
                    permission.Add(item.Key, this.GetMemuFitler(item.Value, new PermissionChildren(item.Value.Name, item.Value.DisplayName, item.Value.Type, item.Value.Meta), permissions));
                }
            }
            return permission.Values.ToImmutableList();
        }
        private PermissionChildren GetMemuFitler(PermissionChildren permission, PermissionChildren children, string[] permissions)
        {
            foreach (var item in permission.Children)
            {
                if (permissions.Contains(item.Name) && item.Type == PermissionType.Memu)
                {
                    this.GetMemuFitler(item, children.CreateChildPermission(item.Name, item.DisplayName, item.Type, item.Meta), permissions);
                }
            }
            return children;
        }

    }
}
