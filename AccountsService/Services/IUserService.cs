﻿using AccountsService.Dtos;

namespace AccountsService.Services
{
    public interface IUserService
    {
        Task<string> AssignAdminRole(string email);
        Task<string> ChangePassword(ChangePasswordRequest request, string userId);
        Task<string> DeleteUser(string email);
        Task<List<GetUserResponse>> GetAllAdmins();
        Task<List<GetUserResponse>> GetAllUsers();
        Task<GetUserResponse> GetUser(GetUserRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task<CreateUserResponse> RegisterUser(CreateUserRequest request);
        Task<GetUserResponse> UpdateUser(UpdateUserRequest request, string userId);
    }
}