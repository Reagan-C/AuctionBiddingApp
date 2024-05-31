using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Interface;
using UserService.Models;
using UserService.Protos;
using UserService.Validations;

namespace UserService.Services
{
    public class UserAccountServiceImpl : UserAccountService.UserAccountServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserAccountServiceImpl(IConfiguration configuration, IUserRepository userRepository, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public override async Task<CreateUserResponse> RegisterUser(CreateUserRequest request, ServerCallContext context)
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

                throw new RpcException(new Status(StatusCode.InvalidArgument, "Error encountered"));
            }
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
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
            return await Task.FromResult( new LoginResponse { Token = token, Username = user.UserName });
        }

        [Authorize]
        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
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
        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
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
        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
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
        public override async Task<AssignRoleResponse> AssignAdminRole(AssignRoleRequest request, ServerCallContext context)
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


            return new AssignRoleResponse { Response = $"{user.Name} has been made an {role}"};
        }

        [Authorize]
        public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
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

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:Secret"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["JwtOptions:Issuer"],
                Audience = _configuration["JwtOptions:Audience"],
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
