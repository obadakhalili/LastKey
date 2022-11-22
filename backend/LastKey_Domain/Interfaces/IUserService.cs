using LastKey_Domain.Entities.DTOs;

namespace LastKey_Domain.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserRequest request);
}