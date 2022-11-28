using LastKey_Domain.Entities.DTOs;

namespace LastKey_Domain.Interfaces;

public interface ILockService
{
    Task<Lock?> RegisterLockAsync(LockPairRequest request);

    Task<bool> UnpairLockAsync(LockUnpairRequest request);

    Task<List<Lock>?> RetrieveUserLocksAsync(int userId);

    Task<Lock?> UpdateLockNameAsync(UpdateLockRequest request);
}