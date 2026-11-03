using Complete_Login_System.Services.Intefaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Complete_Login_System.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? UserId => _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(JwtRegisteredClaimNames.Sub);

        public string? Email => _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(JwtRegisteredClaimNames.Email);
    }
}
