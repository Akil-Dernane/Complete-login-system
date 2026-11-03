using Complete_Login_System.DTOs.Auth;

namespace Complete_Login_System.Services.Intefaces
{
    public interface IAuthService
    {
        //register
        Task<RegisterResponseDto> Register(RegisterRequestDto dto);
        //login
        Task<AuthResponseDto> Login(LoginDto dto);
        //refresh
        Task<AuthResponseDto> RefreshTokens(RefreshRequestDto dto);
        //changePassword
        Task ChangePasswordAsync(ChangePasswordRequestDto dto);
        //logout
        Task LogoutAsync(RefreshRequestDto dto);
        //logoutall
        Task LogoutAllAsync();

    }
}
