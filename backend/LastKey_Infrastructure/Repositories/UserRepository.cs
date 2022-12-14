using LastKey_Domain.Entities;
using LastKey_Domain.Interfaces;
using LastKey_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetUserInfoByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username);
    }

    public List<User> RetrieveMembersByUserId(int userId)
    {
        return _context.Users.Where(u => u.AdminId == userId).ToList();
    }

    public async Task<bool> DeleteUserAsync(int userId, int adminId)
    {
        var userToDelete = await _context.Users
            .FirstAsync(u => u.UserId == userId && u.AdminId == adminId);

        try
        {
            _context.Users.Remove(userToDelete);
            
            await _context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}