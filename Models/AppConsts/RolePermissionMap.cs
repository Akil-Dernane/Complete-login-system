namespace Complete_Login_System.Models.AppConsts
{
    public static class RolePermissionMap
    {
        private static readonly string[] UserPermission =
        {
        };

        private static readonly string[] ModeratorPermission =
        {
        };

        private static readonly string[] AdminPermission =
        {

        };

        public static readonly Dictionary<string, string[]> Defaults = new()
        {
            [Roles.User] = UserPermission,
            [Roles.Moderator] = ModeratorPermission,
            [Roles.Admin] = AdminPermission,
        };
    }
}
