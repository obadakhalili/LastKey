using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using LastKey_Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Authorize]
[Controller]
[Route("api/locks")]
public class LockController : ControllerBase
{
    private readonly ILockService _lockService;

    public LockController(ILockService lockService)
    {
        _lockService = lockService;
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
    public async Task<ActionResult<Lock>> UpdateLockName(int lockId, [FromBody] UpdateLockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest();
        }
        
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var updatedLock = await _lockService.UpdateLockNameAsync(lockId, request.Name, userId);

        if (updatedLock == null)
        {
            return BadRequest(new
            {
                message = "The name chosen already in use"
            });
        }

        return Ok();
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