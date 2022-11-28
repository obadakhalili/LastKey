using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using Lock = LastKey_Domain.Entities.Lock;

namespace LastKey_Web.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, LastKey_Domain.Entities.User>();

        CreateMap<LastKey_Domain.Entities.User, User>();

        CreateMap<Lock, LastKey_Domain.Entities.DTOs.Lock>();

        CreateMap<LockPairRequest, Lock>()
            .ForMember((d) => d.LockName,
                op => op.MapFrom(s => s.AdminId))
            .ForMember(d => d.MacAddress,
                op => op.MapFrom(s => s.LockMacAddress))
            .ForMember(d => d.UserId,
                op => op.MapFrom(s => s.AdminId));
    }
}