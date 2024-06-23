namespace ApiGateway.Services
{
    public interface IUserService
    {
        Task<AssignRoleResponse> AssignAdminRole(AssignRoleRequest request);
        Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request);
        Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request);
        Task<GetAdminResponse> GetAllAdmins(GetAllRequest request);
        Task<GetAllResponse> GetAllUsers(GetAllRequest request);
        Task<GetUserResponse> GetUser(GetUserRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task<CreateUserResponse> RegisterUser(CreateUserRequest request);
        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);
    }
}