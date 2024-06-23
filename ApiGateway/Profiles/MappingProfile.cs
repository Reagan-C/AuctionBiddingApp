using AutoMapper;

namespace ApiGateway.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<CreateUserRequest, ApplicationUser>();
            CreateMap<ApplicationUser, GetUserResponse>();
            CreateMap<ApplicationUser, UpdateUserResponse>();
           
        }
    }
}
