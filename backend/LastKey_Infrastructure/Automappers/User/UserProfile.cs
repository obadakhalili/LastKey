using AutoMapper;

namespace LastKey_Infrastructure.Automappers.User;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<LastKey_Domain.Models.User.User, Models.User.User>();
        CreateMap<Models.User.User, LastKey_Domain.Models.User.User>();
    }
}