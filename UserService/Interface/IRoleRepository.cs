using UserService.Models;

namespace UserService.Interface
{
    public interface IRoleRepository
    {
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    }
}
