using System.Security;

namespace Complete_Login_System.Models.AppConsts
{
    public static class Claims
    {
        public const string Permmision = "permission";

        public static readonly IReadOnlySet<string> RoleClaimTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                Permmision,
            };
        public static readonly IReadOnlySet<string> UserClaimTypes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                Permmision,
            };
    }
}
