using Complete_Login_System.Models.AppConsts;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Complete_Login_System.Models.Perrmissions
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.HasClaim(Claims.Permmision, requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
