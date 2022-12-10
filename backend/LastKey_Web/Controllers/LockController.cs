using AutoMapper;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using LastKey_Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Authorize]
[Controller]
[Route("api/locks")]
public class LockController : ControllerBase
{
    private readonly ILockService _lockService;
    private readonly IMapper _mapper;

    public LockController(ILockService lockService, IMapper mapper)
    {
        _lockService = lockService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<Lock>> PairLockToUser([FromBody] LockPairRequest request)
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var createdLock = await _lockService.RegisterLockAsync(request, userId);

        if (createdLock == null)
            return BadRequest(new {
                message = $"A lock with the name \"{request.LockName}\" already exists!"
            });

        return createdLock;
    }

    [HttpDelete("{lockId}")]
    public async Task<ActionResult> UnpairLock([FromRoute] int lockId)
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var isDeleted = await _lockService.UnpairLockAsync(lockId, userId);

        if (isDeleted == false)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<Lock>>> RetrieveLocksForUser()
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var userLocks = await _lockService.RetrieveUserLocksAsync(userId);

        return Ok(userLocks);
    }

    [HttpPatch("{lockId}")]
    public async Task<ActionResult<Lock>> UpdateLock(int lockId, 
        [FromBody] JsonPatchDocument<LastKey_Domain.Entities.Lock> patchDocument)
    {
        var request = new UpdateLockRequest
        {
            LockId = lockId,
            UserId = JwtSecurityHelper.GetUserIdFromToken(Request)
        };

        var updatedLock = await _lockService.UpdateLockAsync(request, patchDocument);

        if (updatedLock == null)
            return BadRequest(new
            {
                message = "Lock wasn't found or doesn't belong to this user!"
            });

        return Ok(updatedLock);
    }

    [AllowAnonymous]
    [HttpGet("{macAddress}")]
    public async Task<ActionResult<bool>> LockExists(string macAddress)
    {
        return Ok(await _lockService.LockExistsAsync(macAddress));
    }

    [AllowAnonymous]
    [HttpGet("state/{macAddress}")]
    public async Task<ActionResult<bool>> GetLockState(string macAddress)
    {
        return Ok(await _lockService.RetrieveLockStateAsync(macAddress));
    }
}