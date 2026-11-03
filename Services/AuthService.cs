using Complete_Login_System.Data;
using Complete_Login_System.DTOs.Auth;
using Complete_Login_System.Exceptions;
using Complete_Login_System.Exceptions.Auth;
using Complete_Login_System.Exceptions.User;
using Complete_Login_System.Models;
using Complete_Login_System.Models.AppConsts;
using Complete_Login_System.Services.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Complete_Login_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JwtSettings _jwt;
        private readonly ITokenService _tokens;
        SignInManager <ApplicationUser> _signInManager;
        public AuthService(
            AppDbContext context,
            ICurrentUserService currentUser,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOptions<JwtSettings> jwtSettings,
            ITokenService tokens,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _context = context;
            _currentUser = currentUser;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwtSettings.Value;
            _tokens = tokens;
            _signInManager = signInManager;
        }

        public async Task<RegisterResponseDto> Register(RegisterRequestDto dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var result = await _userManager.CreateAsync(user, dto.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors;
                if (errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
                {
                    throw new ConflictException("An account with this username or email already exists.");
                }
                throw new RegistrationFailedException(errors.Select(e => e.Description));
            }

            var roleResault = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!roleResault.Succeeded)
            {
                throw new RegistrationFailedException(roleResault.Errors.Select(e => e.Description));
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new RegisterResponseDto(user.Email);
        }
        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);

            if(user == null)
            {
                //Audit
                throw new InvalidCredentialsException();
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);

            if (result.IsLockedOut)
            {
                //Audit
                throw new AccountLockedException(await _userManager.GetLockoutEndDateAsync(user));
            }

            if(!result.Succeeded)
            {
                //Audit
                throw new InvalidCredentialsException();
            }

            var (entity ,raw) = NewRefreshToken(user.Id,Guid.NewGuid());
            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync();

            var (access, accessExpires) = await BuildAccessToken(user);

            return new AuthResponseDto(access, accessExpires, raw, entity.ExpiresAtUtc);
        }
        private async Task <(string token, DateTime ExpiresAtUtc)> BuildAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>( await _userManager.GetClaimsAsync(user));

            var roleNames = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim("role", roleName));

                var role = await _roleManager.FindByNameAsync(roleName);

                if (role is null)
                    continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claims.AddRange(roleClaims);
            }

            string token = _tokens.CreateAccessToken(user.Id, user.Email!, claims.DistinctBy(c => (c.Type, c.Value)));
            return (token, DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes));
        }
        private (RefreshToken Entity, string raw) NewRefreshToken(Guid userId, Guid FamilyId)
        {
            string raw = RefreshTokenGenerator.Generate();
            var now = DateTime.UtcNow;

            var Entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = RefreshTokenGenerator.Hash(raw),
                FamilyId = FamilyId,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(_jwt.RefreshTokenDays)
            };

            return(Entity, raw);
        }
        public async Task<AuthResponseDto> RefreshTokens(RefreshRequestDto dto)
        {
            var hash = RefreshTokenGenerator.Hash(dto.RefreshToken);
            DateTime now = DateTime.UtcNow;


            RefreshToken stored = await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == hash)
                ?? throw new InvalidRefreshTokenException();

            //string? actorIdentifier =
            //stored.User.Email?.Trim().ToUpperInvariant();

            if(stored.RevokedAtUtc is not null)
            {
                if(stored.ReplacedByTokenHash is not null)
                {
                    await RevokeFamilyAsync(stored.FamilyId, now);
                    //Audit
                }

                throw new InvalidRefreshTokenException();
            }

            if(stored.ExpiresAtUtc <= now)
            {
                //Audit
                throw new InvalidRefreshTokenException();
            }

            if(await _userManager.IsLockedOutAsync(stored.User))
            {
                //Audit
                throw new InvalidRefreshTokenException();
            }

            var (next, raw) = NewRefreshToken(
                stored.UserId, 
                stored.FamilyId
            );
            await using var tx = await _context.Database.BeginTransactionAsync();

            int claimed = await _context.RefreshTokens
                .Where(t => t.Id == stored.Id && t.RevokedAtUtc == null)
                .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAtUtc, now)
                .SetProperty(t => t.ReplacedByTokenHash, next.TokenHash));

            if(claimed == 0)
            {
                await tx.RollbackAsync();

                var latest = await _context.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == stored.Id);
                if(latest?.ReplacedByTokenHash is not null)
                {
                    await RevokeFamilyAsync(stored.FamilyId, now);
                    //Audit reuse

                }

                throw new InvalidRefreshTokenException();
            }

            _context.RefreshTokens.Add(next);

            // Record successful refresh.

            await _context.SaveChangesAsync();

            var (access, accessExpires) = await BuildAccessToken(stored.User);
            await tx.CommitAsync();

            return new AuthResponseDto(
                access,
                accessExpires,
                raw,
                next.ExpiresAtUtc
            );
        }
        private async Task<int> RevokeFamilyAsync(Guid familyId, DateTime now)
        {
            return await _context.RefreshTokens
                .Where(t => t.FamilyId == familyId && t.RevokedAtUtc == null)
                .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAtUtc, now));
        }


        public async Task ChangePasswordAsync(ChangePasswordRequestDto dto)
        {

            Guid.TryParse(_currentUser.UserId, out var userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new UserNotFoundException(userId);


            IdentityResult result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                throw new PasswordChangeFailedException(errors);
            }
        }
        public async Task LogoutAsync(RefreshRequestDto dto)
        {
            var hash = RefreshTokenGenerator.Hash(dto.RefreshToken);
            var now = DateTime.UtcNow;

            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
            if (stored == null || stored.RevokedAtUtc is not null) return;

            int revoked = await RevokeFamilyAsync(stored.FamilyId, now);
            if(revoked == 0) return;

            //Audit
        }
        public async Task LogoutAllAsync()
        {
            Guid.TryParse(_currentUser.UserId, out var userId);
            var now = DateTime.UtcNow;

            int revoked =
                await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.RevokedAtUtc, now));

            if(revoked == 0) return;

            //Audit
        }
    }
}
