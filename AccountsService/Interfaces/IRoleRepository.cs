using AccountsService.Models;

namespace AccountsService.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    }
}
