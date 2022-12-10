using LastKey_Domain.Entities.DTOs;
using Lock = LastKey_Domain.Entities.Lock;

namespace LastKey_Domain.Interfaces;

public interface ILockRepository
{
    Task AddLockAsync(int userId, Lock @lock);

    Task<bool> LockNameExistsForUserAsync(string lockName, int userId, int? lockId);

    Task<bool> DeleteLockForUserAsync(int userId, int lockId);

    Task<List<Lock>> RetrieveLocksForUserAsync(int userId);

    Task<bool> LockMacAddressExistsAsync(string macAddress);

    Task<bool> GetLockStateAsync(string macAddress);

    Task<Lock?> UpdateLockAsync(UpdateLockRequest request);
}