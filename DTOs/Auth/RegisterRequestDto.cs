using System.ComponentModel.DataAnnotations;

namespace Complete_Login_System.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
