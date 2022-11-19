using LastKey_Application.DTOs.User;

namespace LastKey_Application.Services.User;

public interface IUserService
{
    Task<LastKey_Domain.Models.User.User> CreateUserAsync(LastKey_Domain.Models.User.User user);
}