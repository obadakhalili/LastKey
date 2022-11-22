using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;

namespace LastKey_Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var userModel = _mapper.Map<LastKey_Domain.Entities.User>(request);
        
        userModel = userModel with
        {
            IsAdmin = true
        };
        

        return _mapper.Map<User>(await _userRepository.CreateUserAsync(userModel));
    }
}