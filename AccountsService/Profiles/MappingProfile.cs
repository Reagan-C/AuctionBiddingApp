using AccountsService.Dtos;
using AccountsService.Models;
using AutoMapper;

namespace AccountsService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<CreateUserRequest, ApplicationUser>();
            CreateMap<ApplicationUser, GetUserResponse>();
           
        }
    }
}
