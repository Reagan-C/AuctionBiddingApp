using ApiGateway.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UserService.Interface;

namespace ApiGateway.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, IRoleRepository roleRepository)
        {
            _userRepository=userRepository;
            _userManager=userManager;
            _roleManager=roleManager;
            _mapper=mapper;
            _roleRepository=roleRepository;
        }

        public async Task<CreateUserResponse> RegisterUser(CreateUserRequest request)
        {
            var validator = new CreateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var user = _mapper.Map<ApplicationUser>(request);
            user.UserName = request.Email;

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    var assignRole = await _userManager.AddToRoleAsync(user, "User");
                    if (!assignRole.Succeeded)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, assignRole.Errors.FirstOrDefault().Description));
                    }
                    return await Task.FromResult(new CreateUserResponse
                    {
                        Message = "User registered successfully"
                    });
                }
                throw new RpcException(new Status(StatusCode.InvalidArgument, result.Errors.FirstOrDefault().Description));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var user = await _userRepository.GetUserAsync(request.Email);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"user with email {request.Email} not found"));

            bool isValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValid)
                throw new RpcException(new Status(StatusCode.NotFound, "username/password incorrect"));

            var token = await GenerateJwtToken(user);
            return await Task.FromResult(new LoginResponse { Token = token, Username = user.UserName });
        }

        [Authorize]
        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            var validator = new GetUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var user = await _userRepository.GetUserAsync(request.Email);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "user not found"));

            var userResponse = _mapper.Map<GetUserResponse>(user);
            return await Task.FromResult(userResponse);
        }

        [Authorize]
        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            var validator = new UpdateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var userId = context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Error encountered while getting user Id"));

            await _userRepository.UpdateUserAsync(request, user);

            var updateUserResponse = _mapper.Map<UpdateUserResponse>(user);

            return await Task.FromResult(updateUserResponse);
        }

        [Authorize(Roles = "Admin")]
        public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request)
        {
            var validator = new DeleteUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var result = await _userRepository.DeleteUserAsync(request);
            if (!result)
                throw new RpcException(new Status(StatusCode.NotFound, "user profile not found"));

            return new DeleteUserResponse { Response = "User deleted successfully" };
        }

        [Authorize(Roles = "Admin")]
        public async Task<AssignRoleResponse> AssignAdminRole(AssignRoleRequest request)
        {
            var validator = new AssignRoleRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var role = "Admin";
            var user = await _userRepository.GetUserAsync(request.Email);
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (user == null || !roleExists)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "user/role not found"));
            }

            await _userManager.AddToRoleAsync(user, role);


            return new AssignRoleResponse { Response = $"{user.Name} has been made an {role}" };
        }

        [Authorize]
        public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        {
            var validator = new ChangePasswordRequestValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMessages));
            }

            var userId = context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "user not found"));

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
                throw new RpcException(new Status(StatusCode.Internal, result.Errors.FirstOrDefault().Description));

            return await Task.FromResult(new ChangePasswordResponse { Response = "Password changed" });
        }

        [Authorize(Roles = "Admin")]
        public async Task<GetAllResponse> GetAllUsers(GetAllRequest request)
        {
            var response = new GetAllResponse();
            var users = await _userRepository.GetAllUsers();

            foreach (var user in users)
            {
                response.Users.Add(new GetUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.UserName
                });
            }

            return response;
        }

        [Authorize(Roles = "Admin")]
        public async Task<GetAdminResponse> GetAllAdmins(GetAllRequest request)
        {
            var response = new GetAdminResponse();
            var adminUsers = await _roleRepository.GetUsersByRoleAsync("Admin");

            foreach (var user in adminUsers)
            {
                response.Admins.Add(new GetUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.UserName
                });
            }

            return response;
        }

    }
}
