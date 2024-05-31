using UserService.Models;
using UserService.Protos;

namespace UserService.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task CreateUserAsync(CreateUserRequest request);
        Task<bool> UpdateUserAsync(UpdateUserRequest request, ApplicationUser user);
        Task<bool> DeleteUserAsync(DeleteUserRequest request);
        Task<List<ApplicationUser>> GetAllUsers();
    }
}
