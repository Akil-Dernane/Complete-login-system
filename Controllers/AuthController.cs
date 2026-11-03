using Complete_Login_System.DTOs.Auth;
using Complete_Login_System.Services.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Complete_Login_System.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            return await _authService.Login(dto);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto dto)
        {
            return await _authService.Register(dto);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> RefreshTokens(RefreshRequestDto dto)
        {
            return await _authService.RefreshTokens(dto);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto dto)
        {
            await _authService.ChangePasswordAsync(dto);
            return NoContent();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto);
            return NoContent();
        }
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            await _authService.LogoutAllAsync();
            return NoContent();
        }
    }
}
