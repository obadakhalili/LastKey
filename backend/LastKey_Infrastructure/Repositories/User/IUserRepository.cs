namespace LastKey_Infrastructure.Repositories.User;

public interface IUserRepository
{
    Task<Models.User.User> CreateUserAsync(Models.User.User user);
}