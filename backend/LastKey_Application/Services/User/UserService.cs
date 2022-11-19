using AutoMapper;
using LastKey_Application.DTOs.User;
using LastKey_Infrastructure.Repositories.User;

namespace LastKey_Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<LastKey_Domain.Models.User.User> CreateUserAsync(LastKey_Domain.Models.User.User user)
    {
        user = user with
        {
            IsAdmin = true
        };

        var userEntity = _mapper.Map<LastKey_Infrastructure.Models.User.User>(user);

        var createdUser = await _userRepository.CreateUserAsync(userEntity);

        return _mapper.Map<LastKey_Domain.Models.User.User>(createdUser);
    }
}