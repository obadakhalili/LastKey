using AutoMapper;
using LastKey_Application.DTOs.User;

namespace LastKey_Application.Automappers.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserForCreationDto, LastKey_Domain.Models.User.User>();

        CreateMap<LastKey_Domain.Models.User.User, UserDto>();
    }
}