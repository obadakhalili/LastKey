using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace LastKey_Application.Services;

public class LockService : ILockService
{
    private readonly ILockRepository _lockRepository;
    private readonly IMapper _mapper;

    public LockService(ILockRepository lockRepository, IMapper mapper)
    {
        _lockRepository = lockRepository;
        _mapper = mapper;
    }
    
    public async Task<Lock?> RegisterLockAsync(LockPairRequest request, int adminId)
    {
        if (await _lockRepository.LockNameExistsForUserAsync(request.LockName, adminId, null))
        {
            return null;
        }
        
        var lockToCreate = _mapper.Map<LastKey_Domain.Entities.Lock>(request);

        lockToCreate = lockToCreate with
        {
            IsLocked = true
        };

        await _lockRepository.AddLockAsync(adminId, lockToCreate);

        return _mapper.Map<Lock>(lockToCreate);
    }

    public Task<bool> UnpairLockAsync(int lockId, int adminId)
    {
        return _lockRepository.DeleteLockForUserAsync(adminId, lockId);
    }

    public async Task<List<Lock>> RetrieveUserLocksAsync(int userId)
    {
        var userLocks = await _lockRepository.RetrieveLocksForUserAsync(userId);

        return _mapper.Map<List<Lock>>(userLocks);
    }

    public async Task<bool> LockExistsAsync(string macAddress)
    {
        return await _lockRepository.LockMacAddressExistsAsync(macAddress);
    }

    public async Task<bool> RetrieveLockStateAsync(string macAddress)
    {
        return await _lockRepository.GetLockStateAsync(macAddress);
    }

    public async Task<Lock?> UpdateLockAsync(UpdateLockRequest request,
        JsonPatchDocument<LastKey_Domain.Entities.Lock> patchDocument)
    {
        var updatedLock = await _lockRepository.UpdateLockAsync(request, patchDocument);

        return _mapper.Map<Lock>(updatedLock);
    }
}