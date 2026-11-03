using Complete_Login_System.Models;
using Complete_Login_System.Models.AppConsts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Complete_Login_System.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider service, IConfiguration config)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();

            //Seeding Roles
            foreach(var role in Roles.All)
            {
                if(! await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            //Seeding Role Claims
            foreach( var (roleName,permissions) in RolePermissionMap.Defaults)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role is null)
                    continue;

                var existing = (await roleManager.GetClaimsAsync(role))
                    .Where(c => c.Type == Claims.Permmision)
                    .Select(c => c.Value)
                    .ToHashSet();
                foreach (string permission in permissions.Where(p => !existing.Contains(p)))
                    await roleManager.AddClaimAsync(role, new Claim(Claims.Permmision, permission));
            }
        }
    }
}
