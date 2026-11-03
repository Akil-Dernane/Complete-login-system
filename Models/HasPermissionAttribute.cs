using Microsoft.AspNetCore.Authorization;

namespace Complete_Login_System.Models
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permmision) : base(permmision) {}
    }
}
