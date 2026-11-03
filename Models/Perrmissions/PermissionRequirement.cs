using Microsoft.AspNetCore.Authorization;

namespace Complete_Login_System.Models.Perrmissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }

    }
}
