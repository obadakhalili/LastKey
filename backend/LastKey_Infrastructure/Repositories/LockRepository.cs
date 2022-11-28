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
}