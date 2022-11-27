using LastKey_Domain.Entities.DTOs;

namespace LastKey_Domain.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserRequest request);

    Task<AuthenticationResponse?> AuthenticateUserAsync(LoginUserRequest request);

    Task<User?> RetrieveUserInfoByIdAsync(int userId);

    void ClearCookies();

    Task<bool> UsernameExistsAsync(string username);
}