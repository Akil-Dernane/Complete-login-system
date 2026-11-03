using Complete_Login_System.Models.AppConsts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Complete_Login_System.Models.Perrmissions
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy? existing = await base.GetPolicyAsync(policyName);

            if (existing is not null)
                return existing;

            if (Permissions.All.Contains(policyName))
            {
                return new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }

            return null;

        }
    }
}
