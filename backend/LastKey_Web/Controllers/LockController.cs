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
}