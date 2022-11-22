using LastKey_Domain.Models.User;

namespace LastKey_Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
}