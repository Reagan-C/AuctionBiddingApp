using AccountsService.Dtos;
using AccountsService.Interfaces;
using AccountsService.Models;
using AccountsService.Validations;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, IRoleRepository roleRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
        }

        public async Task<CreateUserResponse> RegisterUser(CreateUserRequest request)
        {
            var validator = new CreateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errorMessages);
            }

            var user = _mapper.Map<ApplicationUser>(request);
            user.UserName = request.Email;


            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault().Description);
            }
            
            var assignRole = await _userManager.AddToRoleAsync(user, "User");
            if (!assignRole.Succeeded)
            {
                throw new Exception(assignRole.Errors.FirstOrDefault().Description);
            }

            return await Task.FromResult(new CreateUserResponse
            {
                Message = "User registered successfully"
            });
        }

        public async Task<LoginResponse> Login(LoginRequest request, string ipAddress)
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errorMessages);
            }

            var user = await _userRepository.GetUserAsync(request.Email);
            if (user == null)
                throw new Exception($"User with email {request.Email} not found");

            bool isValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValid)
                throw new Exception("username/password incorrect");

            var token = await _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(ipAddress);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return await Task.FromResult(new LoginResponse { Token = token, RefreshToken = refreshToken.Token });
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var validator = new GetUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errorMessages);
            }

            var user = await _userRepository.GetUserAsync(request.Email);
            if (user == null)
                throw new Exception("user not found");

            var userResponse = _mapper.Map<GetUserResponse>(user);
            return await Task.FromResult(userResponse);
        }

        public async Task<GetUserResponse> UpdateUser(UpdateUserRequest request, string userId)
        {
            var validator = new UpdateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errorMessages);
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("Error encountered while getting user Id");

            await _userRepository.UpdateUserAsync(request, user);

            var updateUserResponse = _mapper.Map<GetUserResponse>(user);

            return await Task.FromResult(updateUserResponse);
        }

        public async Task<string> DeleteUser(string email)
        {
            var result = await _userRepository.DeleteUserAsync(email);
            if (!result)
                throw new Exception("user profile not found");

            return "User deleted successfully";
        }

        public async Task<string> AssignAdminRole(string email)
        {
            var role = "Admin";
            var user = await _userRepository.GetUserAsync(email);
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (user == null || !roleExists)
            {
                throw new Exception("user/role not found");
            }

            await _userManager.AddToRoleAsync(user, role);


            return $"{user.Email} has been made an {role}";
        }

        public async Task<string> ChangePassword(ChangePasswordRequest request, string userid)
        {
            var validator = new ChangePasswordRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception(errorMessages);
            }

            var user = await _userRepository.GetUserByIdAsync(userid);

            if (user == null)
                throw new Exception("user not found");

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
                throw new Exception(result.Errors.FirstOrDefault().Description);

            return "Password changed";
        }

        public async Task<List<GetUserResponse>> GetAllUsers()
        {
            var response = new List<GetUserResponse>();
            var users = await _userRepository.GetAllUsers();

            return await Task.FromResult(_mapper.Map<List<GetUserResponse>>(users));
        }

        public async Task<List<GetUserResponse>> GetAllAdmins()
        {
            var response = new List<GetUserResponse>();
            var adminUsers = await _roleRepository.GetUsersByRoleAsync("Admin");

            return await Task.FromResult(_mapper.Map<List<GetUserResponse>>(adminUsers));
        }

        public async Task<(RefreshToken, string)> RefreshJwtToken(string refreshToken, string ipAddress)
        {
            var (newRefreshToken, newJwtToken) = await _tokenService.RefreshJwtToken(refreshToken, ipAddress);
            return (newRefreshToken, newJwtToken);
        }

        public async Task Logout(string refreshToken, string ipAddress)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token.Equals(refreshToken)));
            if (user != null)
            {
                var token = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
                if (token == null)
                {
                    return;
                }
                _tokenService.RevokeRefreshToken(token, ipAddress, "Log out operation", "");
                await _userManager.UpdateAsync(user);
            }
            return;
        }
    }
}
