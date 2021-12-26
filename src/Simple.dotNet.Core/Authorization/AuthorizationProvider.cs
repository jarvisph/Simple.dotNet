namespace Simple.dotNet.Core.Authorization
{
    public abstract class AuthorizationProvider
    {
        public abstract void SetPermissions(IPermissionDefinitionContext context);
    }
}
