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
    private readonly IUserService _userService;

    public LockController(ILockService lockService, IUserService userService)
    {
        _lockService = lockService;
        _userService = userService;
    }

    [Authorize(Roles = nameof(Roles.Admin))]
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

    [HttpPatch("{lockId}/name/{name}")]
    public async Task<ActionResult<Lock>> UpdateLockName(int lockId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new
            {
                message = "Please specify a name!"
            });
        }

        var updateRequest = new UpdateLockRequest
        {
            UserId = JwtSecurityHelper.GetUserIdFromToken(Request),
            LockId = lockId,
            PropertyToUpdate = LockProperties.Name,
            NewName = name
        };

        var updatedLock = await _lockService.UpdateLockAsync(updateRequest);

        if (updatedLock == null)
            return BadRequest(new
            {
                message = "Lock name already exists for user or Lock doesn't belong to specified user!"
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

    [HttpPatch("{lockId}/unlock")]
    public async Task<ActionResult> UnlockLock(int lockId, [FromBody] UnlockLockRequest request)
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var user = await _userService.RetrieveUserInfoByIdAsync(userId);

        var client = new HttpClient();

        var freeFaceValues = new Dictionary<string, string>
        {
            {"api_key", Environment.GetEnvironmentVariable("face_api_key")!},
            {"api_secret", Environment.GetEnvironmentVariable("face_api_secret")!},
            {"image_base64_1", user!.UserImage},
            {"image_base64_2", request.Image}
        };

        var freeFaceBody = new FormUrlEncodedContent(freeFaceValues);

        var response = await client.PostAsync("https://api-us.faceplusplus.com/facepp/v3/compare", freeFaceBody);

        var freeFaceResponse = await response.Content.ReadFromJsonAsync<FreeFaceResponse>();

        if (freeFaceResponse.confidence != null && freeFaceResponse.confidence < freeFaceResponse.thresholds.e4)
        {
            return Forbid();
        }
        
        var updateRequest = new UpdateLockRequest
        {
            UserId = user.IsAdmin ? userId : (int) user.AdminId,
            LockId = lockId,
            PropertyToUpdate = LockProperties.LockState,
            IsLocked = false
        };

        var updatedLock = await _lockService.UpdateLockAsync(updateRequest);

        if (updatedLock == null)
        {
            return BadRequest(new
            {
                message = "Lock not found or doesn't belong to user!"
            });
        }

        return Ok(new
        {
            updatedLock,
            freeFaceResponse
        });
    }
    
    [HttpPatch("{lockId}/lock")]
    public async Task<ActionResult> LockLock(int lockId)
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);
        var user = await _userService.RetrieveUserInfoByIdAsync(userId);

        var updateRequest = new UpdateLockRequest
        {
            UserId = user.IsAdmin ? userId : (int) user.AdminId,
            LockId = lockId,
            PropertyToUpdate = LockProperties.LockState,
            IsLocked = true
        };

        var updatedLock = await _lockService.UpdateLockAsync(updateRequest);

        if (updatedLock == null)
        {
            return BadRequest(new
            {
                message = "Lock not found or doesn't belong to user!"
            });
        }

        return Ok(updatedLock);
    }
}