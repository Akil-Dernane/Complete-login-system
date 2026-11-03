using Microsoft.AspNetCore.Identity;

namespace Complete_Login_System.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
