using System.Security.Cryptography;
using System.Text;
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

        var ms = new MemoryStream();

        await request.UserImage.CopyToAsync(ms);

        var userImageFileBytes = ms.ToArray();

        var b64UserImage = Convert.ToBase64String(userImageFileBytes);

        userModel = userModel with
        {
            IsAdmin = true,
            UserImage = b64UserImage,
            Password = GenerateEncryptedPassword(userModel.Password)
        };

        var createdUser = _mapper.Map<User>(await _userRepository.CreateUserAsync(userModel));

        return createdUser;
    }

    private string GenerateEncryptedPassword(string password)
    {
        var sha256 = SHA256.Create();

        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));

        var encryptedPassword = Convert.ToBase64String(hash);

        return encryptedPassword;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.UsernameExistsAsync(username);
    }
}