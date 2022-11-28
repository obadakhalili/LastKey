using LastKey_Domain.Entities;

namespace LastKey_Domain.Interfaces;

public interface ILockRepository
{
    Task<List<Lock>> RetrieveLocksAsync();

    Task AddLockAsync(Lock @lock);

    Task<bool> LockNameExistsForUserAsync(string lockName, int userId);

    Task<bool> DeleteLockForUserAsync(int userId, int lockId);

    Task<List<Lock>?> RetrieveLocksForUserAsync(int userId);
}