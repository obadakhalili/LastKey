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
    
    public async Task AddLockAsync(int userId, Lock @lock)
    {
        var user = await _context.Users.FirstAsync(u => u.UserId == userId);
        
        user.Locks.Add(@lock);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> LockNameExistsForUserAsync(string lockName, int userId, int? lockId)
    {
        var user = await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == userId);
        
        return user.Locks.Any(l => l.LockId != lockId && l.LockName == lockName);
    }

    public async Task<bool> DeleteLockForUserAsync(int userId, int lockId)
    {
        var user = await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == userId);
        
        var @lock = await _context.Locks.FirstAsync(l => l.LockId == lockId);

        var result = user.Locks.Remove(@lock);

        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<List<Lock>> RetrieveLocksForUserAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == userId);

        if (user.Locks.Count == 0)
        {
            var memberAdmin =  await _context.Users.Include(u => u.Locks).FirstAsync(u => u.UserId == user.AdminId);
            return memberAdmin.Locks;
        }

        return user.Locks;
    }
    
    public async Task<bool> LockMacAddressExistsAsync(string macAddress)
    {
        return await _context.Locks.AnyAsync(l => l.MacAddress == macAddress);
    }

    public async Task<bool> GetLockStateAsync(string macAddress)
    {
        return (await _context.Locks.FirstOrDefaultAsync(l => l.MacAddress == macAddress))!
            .IsLocked;
    }

    public async Task<Lock?> UpdateLockAsync(UpdateLockRequest request)
    {
        var user = await _context.Users.Include(u => u.Locks)
            .FirstAsync(u => u.UserId == request.UserId);

        var @lock = user.Locks.FirstOrDefault(l => l.LockId == request.LockId);

        if (@lock == null)
            return null;

        if (request.PropertyToUpdate == LockProperties.Name)
            @lock.LockName = request.NewName;

        if (request.PropertyToUpdate == LockProperties.LockState)
            @lock.IsLocked = request.IsLocked.GetValueOrDefault();

        await _context.SaveChangesAsync();

        return @lock;
    }
}