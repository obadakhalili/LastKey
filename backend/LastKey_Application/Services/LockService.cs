﻿using AutoMapper;
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

        await _lockRepository.AddLockAsync(lockToCreate);

        return _mapper.Map<Lock>(lockToCreate);
    }

    public Task<bool> UnpairLockAsync(LockUnpairRequest request)
    {
        return _lockRepository.DeleteLockForUserAsync(request.UserId, request.LockId);
    }
}