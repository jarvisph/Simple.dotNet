# Simple.dotNet.Core
### Install Package
    Install-Package Simple.dotNet.Core
### 包介绍
* Authorization
* Dapper
* Data
* Dependency
* Domain
* Drawing
* Encryption
* Expressions
* Extensions
* Http
* Hub
* Jobs
* Languages
* Localization
* Logger
* Mapper

> Authorization 权限相关，用于配置后台菜单及功能权限，使用案例如下：

    internal class PermissionProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var auth = context.CreatePermission(PermissionNames.Authorization, "权限管理", PermissionType.Memu, new PermissionMeta() { { "icon", "" } });
            var admin = auth.CreateChildPermission(PermissionNames.Authorization_Admin, "管理员列表", PermissionType.Memu, new PermissionMeta() { { "href", "/auth/admin" }, { "icon", "" } });
            admin.CreateChildPermission(PermissionNames.Authorization_Admin_Create, "创建");
            admin.CreateChildPermission(PermissionNames.Authorization_Admin_Edit, "编辑");
            admin.CreateChildPermission(PermissionNames.Authorization_Admin_Delete, "删除");

            var role = auth.CreateChildPermission(PermissionNames.Authorization_Role, "角色列表", PermissionType.Memu, new PermissionMeta() { { "href", "/auth/role" }, { "icon", "" } });
            role.CreateChildPermission(PermissionNames.Authorization_Role_Create, "创建");
            role.CreateChildPermission(PermissionNames.Authorization_Role_Edit, "编辑");
            role.CreateChildPermission(PermissionNames.Authorization_Role_Delete, "删除");
        }
    }


