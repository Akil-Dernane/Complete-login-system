using System.ComponentModel.DataAnnotations;

namespace Complete_Login_System.DTOs.Auth
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
