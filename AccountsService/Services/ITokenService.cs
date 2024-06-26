using AccountsService.Models;

namespace AccountsService.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        Task<RefreshToken> GenerateRefreshToken(string ipAddress);
        Task<(RefreshToken, string)> RefreshJwtToken(string refreshToken, string ipAddress);
        void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason, string replacedByToken);
    }
}
