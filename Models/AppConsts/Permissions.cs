using System.Reflection;

namespace Complete_Login_System.Models.AppConsts
{
    public static class Permissions
    {
        public static class User
        {
            public const string Manage = "user.manage";
        }
        public static class AuditLogs
        {
            public const string ReadAll = "audit-logs.read.all";
        }
        public static IReadOnlyList<string> All { get; } = typeof(Permissions)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
    }
}
