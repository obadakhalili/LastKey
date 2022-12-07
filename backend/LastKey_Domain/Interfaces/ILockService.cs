﻿using LastKey_Domain.Entities.DTOs;

namespace LastKey_Domain.Interfaces;

public interface ILockService
{
    Task<Lock?> RegisterLockAsync(LockPairRequest request, int adminId);

    Task<bool> UnpairLockAsync(int lockId, int adminId);

    Task<List<Lock>> RetrieveUserLocksAsync(int userId);

    Task<Lock?> UpdateLockNameAsync(int lockId, string name, int userId);

    Task<bool> LockExistsAsync(string macAddress);

    Task<bool> RetrieveLockStateAsync(string macAddress);
}