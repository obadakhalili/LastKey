using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;

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

    public async Task<Lock?> RegisterLockAsync(LockPairRequest request)
    {
        if (await _lockRepository.LockNameExistsForUserAsync(request.LockName, request.AdminId))
        {
            return null;
        }
        
        var lockToCreate = _mapper.Map<LastKey_Domain.Entities.Lock>(request);

        lockToCreate = lockToCreate with
        {
            IsLocked = true
        };

        await _lockRepository.AddLockAsync(request.AdminId ,lockToCreate);

        return _mapper.Map<Lock>(lockToCreate);
    }

    public Task<bool> UnpairLockAsync(LockUnpairRequest request)
    {
        return _lockRepository.DeleteLockForUserAsync(request.AdminId, request.LockId);
    }

    public async Task<List<Lock>> RetrieveUserLocksAsync(int userId)
    {
        var userLocks = await _lockRepository.RetrieveLocksForUserAsync(userId);

        return _mapper.Map<List<Lock>>(userLocks);
    }

    public async Task<Lock?> UpdateLockNameAsync(UpdateLockRequest request)
    {
        if (await _lockRepository.LockNameExistsForUserAsync(request.NewName, request.AdminId))
            return null;

        var updatedLock = await _lockRepository.UpdateLockNameAsync(request);

        return _mapper.Map<Lock>(updatedLock);
    }

    public async Task<bool> LockExistsAsync(string macAddress)
    {
        return await _lockRepository.LockMacAddressExistsAsync(macAddress);
    }

    public async Task<bool> RetrieveLockStateAsync(string macAddress)
    {
        return await _lockRepository.GetLockStateAsync(macAddress);
    }
}