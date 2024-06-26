using AccountsService.Dtos;
using AccountsService.Models;

namespace AccountsService.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task CreateUserAsync(CreateUserRequest request);
        Task<bool> UpdateUserAsync(UpdateUserRequest request, ApplicationUser user);
        Task<bool> DeleteUserAsync(string email);
        Task<List<ApplicationUser>> GetAllUsers();
    }
}
