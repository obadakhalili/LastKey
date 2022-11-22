using LastKey_Domain.Entities;
using LastKey_Domain.Interfaces;
using LastKey_Infrastructure.Data;

namespace LastKey_Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LastKeyContext _context;

    public UserRepository(LastKeyContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }
}