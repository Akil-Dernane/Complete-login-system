using System.ComponentModel.DataAnnotations;

namespace Complete_Login_System.DTOs
{
    public class ClaimRequestDto
    {
        [Required]
        public string Type = string .Empty;
        [Required]
        public string Value = string .Empty;
    }
}
