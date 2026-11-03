namespace Complete_Login_System.DTOs.Auth
{
    public record AuthResponseDto(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresAtUtc);
}
