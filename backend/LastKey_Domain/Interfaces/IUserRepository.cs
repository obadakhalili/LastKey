using LastKey_Domain.Entities;

namespace LastKey_Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
}