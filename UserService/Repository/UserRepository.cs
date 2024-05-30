using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Interface;
using UserService.Models;
using UserService.Protos;

namespace UserService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context=context;
            _mapper=mapper;
        }

        public async Task CreateUserAsync(CreateUserRequest request)
        {
            var user = _mapper.Map<ApplicationUser>(request);
            user.UserName = request.Email;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(DeleteUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Username.ToLower());
            if (user == null)
            {
                return false;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return user;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserRequest request, ApplicationUser existingUser)
        {
            existingUser.Name = request.Name;
            existingUser.PhoneNumber = request.PhoneNumber;
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
