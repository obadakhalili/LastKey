using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
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

    [HttpPost("pair")]
    public async Task<ActionResult<Lock>> PairLockToUser(LockPairRequest request)
    {
        var createdLock = await _lockService.RegisterLockAsync(request);

        if (createdLock == null)
            return BadRequest($"A lock with the name {request.LockName} already exists!");

        return createdLock;
    }

    [HttpDelete("unpair")]
    public async Task<ActionResult> UnpairLock(LockUnpairRequest request)
    {
        var isDeleted = await _lockService.UnpairLockAsync(request);

        if (isDeleted == false)
            return BadRequest("The specified lock for that user was not found!");

        return NoContent();
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<List<Lock>>> RetrieveLocksForUser(int userId)
    {
        var userLocks = await _lockService.RetrieveUserLocksAsync(userId);

        return Ok(userLocks);
    }

    [HttpPost("updateLock")]
    public async Task<ActionResult<Lock>> UpdateLockName([FromBody] UpdateLockRequest request)
    {
        var response = new Lock();

        if (string.IsNullOrWhiteSpace(request.NewName))
            return BadRequest("Please specify the lock's name");

        var updatedLock = await _lockService.UpdateLockNameAsync(request);

        if (updatedLock == null)
            return BadRequest("The name chosen already exists for user!");

        return updatedLock;
    }

    [AllowAnonymous]
    [HttpGet("{macAddress}")]
    public async Task<ActionResult<bool>> LockExists(string macAddress)
    {
        return Ok(await _lockService.LockExistsAsync(macAddress));
    }
}