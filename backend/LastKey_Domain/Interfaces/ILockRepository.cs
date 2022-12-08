using LastKey_Domain.Entities.DTOs;
using Lock = LastKey_Domain.Entities.Lock;

namespace LastKey_Domain.Interfaces;

public interface ILockRepository
{
    Task<List<Lock>> RetrieveLocksAsync();

    Task AddLockAsync(int userId, Lock @lock);

    Task<bool> LockNameExistsForUserAsync(string lockName, int userId, int? lockId);

    Task<bool> DeleteLockForUserAsync(int userId, int lockId);

    Task<List<Lock>> RetrieveLocksForUserAsync(int userId);

    Task<Lock> UpdateLockNameAsync(int lockId, string name);

    Task<bool> LockMacAddressExistsAsync(string macAddress);

    Task<bool> GetLockStateAsync(string macAddress);
}