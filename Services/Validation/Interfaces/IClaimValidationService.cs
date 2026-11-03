using Complete_Login_System.DTOs;

namespace Library_API.Services.Validation.Interfaces
{
    public interface IClaimValidationService
    {
        void Validate(ClaimRequestDto dto, IReadOnlySet<string> allowedClaimTypes);
    }
}
