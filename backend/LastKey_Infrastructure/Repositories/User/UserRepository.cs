namespace LastKey_Infrastructure.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly LastKeyContext _context;

    public UserRepository(LastKeyContext context)
    {
        _context = context;
    }

    public async Task<Models.User.User> CreateUserAsync(Models.User.User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }
}