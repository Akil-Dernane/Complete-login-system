using System.ComponentModel.DataAnnotations;

namespace Complete_Login_System.DTOs.Auth
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
