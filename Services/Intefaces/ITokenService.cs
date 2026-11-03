using System.Security.Claims;

namespace Complete_Login_System.Services.Intefaces
{
    public interface ITokenService
    {
        string CreateAccessToken(Guid userId, string email, IEnumerable<Claim> claims);
    }
}
