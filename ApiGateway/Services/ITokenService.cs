using ApiGateway.Models;

namespace ApiGateway.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
