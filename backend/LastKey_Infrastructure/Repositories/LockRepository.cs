using LastKey_Domain.Entities;
using LastKey_Domain.Interfaces;
using LastKey_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LastKey_Infrastructure.Repositories;

public class LockRepository : ILockRepository
{
    private readonly LastKeyContext _context;

    public LockRepository(LastKeyContext context)
    {
        _context = context;
    }
    
    public async Task<List<Lock>> RetrieveLocksAsync()
    {
        return await _context.Locks.ToListAsync();
    }

    public async Task AddLockAsync(Lock @lock)
    {
        await _context.Locks.AddAsync(@lock);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> LockNameExistsForUserAsync(string lockName, int userId)
    {
        var user = await _context.Users.FirstAsync(u => u.UserId == userId);
        
        return user.Locks.Any(l => l.LockName == lockName);
    }

    public async Task<bool> DeleteLockForUserAsync(int userId, int lockId)
    {
        var user = await _context.Users.FirstAsync(u => u.UserId == userId);
        
        var @lock = await _context.Locks.FirstAsync(l => l.LockId == lockId);

        return user.Locks.Remove(@lock);
    }
}