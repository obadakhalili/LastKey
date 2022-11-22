using AutoMapper;
using LastKey_Domain.Entities.DTOs;

namespace LastKey_Web.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, LastKey_Domain.Entities.User>();

        CreateMap<LastKey_Domain.Entities.User, User>();
    }
}