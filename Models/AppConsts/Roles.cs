namespace Complete_Login_System.Models.AppConsts
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";

        public static IReadOnlyList<string> All { get; } =
        [
            Admin,
            Moderator,
            User
        ];
    }
}
