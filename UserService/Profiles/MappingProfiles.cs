using AutoMapper;
using UserService.Models;
using UserService.Protos;

namespace UserService.Profiles
{
    public class MappingProfiles : Profile
    {
       public MappingProfiles() 
        {
            CreateMap<CreateUserRequest, ApplicationUser>();
            CreateMap<ApplicationUser, GetUserResponse>();
            CreateMap<ApplicationUser, UpdateUserResponse>();
        }
    }
}
