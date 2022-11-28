using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using LastKey_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Lock = LastKey_Domain.Entities.Lock;

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

    public async Task AddLockAsync(int userId, Lock @lock)
    {
        var user = await _context.Users.FirstAsync(u => u.UserId == userId);
        
        user.Locks.Add(@lock);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> LockNameExistsForUserAsync(string lockName, int userId)
    {
        var user = await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == userId);
        
        return user.Locks.Any(l => l.LockName == lockName);
    }

    public async Task<bool> DeleteLockForUserAsync(int userId, int lockId)
    {
        var user = await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == userId);
        
        var @lock = await _context.Locks.FirstAsync(l => l.LockId == lockId);

        return user.Locks.Remove(@lock);
    }

    public async Task<List<Lock>> RetrieveLocksForUserAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.Locks)
            .FirstAsync(u => u.UserId == userId);

        return user.Locks;
    }

    public async Task<Lock> UpdateLockNameAsync(UpdateLockRequest request)
    {
        var user = await _context.Users.Include(u => u.Locks)
            .FirstAsync(u => u.UserId == request.AdminId);

        var @lock = user.Locks.First(l => l.LockId == request.LockId);

        @lock.LockName = request.NewName;

        await _context.SaveChangesAsync();

        return @lock;
    }
}