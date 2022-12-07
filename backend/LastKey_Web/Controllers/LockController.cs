using System.IdentityModel.Tokens.Jwt;
using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

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

    [HttpPost()]
    public async Task<ActionResult<Lock>> PairLockToUser([FromBody] LockPairRequest request)
    {
        var token = GetToken(Request);
        var userId = GetUserIdFromToken(token);

        var createdLock = await _lockService.RegisterLockAsync(request, userId);

        if (createdLock == null)
            return BadRequest(new {
                message = $"A lock with the name {request.LockName} already exists!"
            });

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

    [HttpPost("{lockId}")]
    public async Task<ActionResult<Lock>> UpdateLockName(int lockId, [FromBody] string name)
    {
        var response = new Lock();

        if (string.IsNullOrWhiteSpace(name))
        {
            response.Message = "Please specify the lock's name";
            return BadRequest(response);
        }

        var token = GetToken(Request);
        var userId = GetUserIdFromToken(token);

        var updatedLock = await _lockService.UpdateLockNameAsync(lockId, name, userId);

        if (updatedLock == null)
        {
            response.Message = "The name chosen already exists for user!";
            return BadRequest(response);
        }
        return updatedLock;
    }

    private int GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jsonToken = handler.ReadJwtToken(token);
        var userId = jsonToken.Claims.First(claim => claim.Type == "userId")
            .Value;

        return Int32.Parse(userId);
    }

    private string GetToken(HttpRequest request)
    {
        request.Cookies.TryGetValue("jwtHeader", out var jwtHeader);
        request.Cookies.TryGetValue("jwtPayload", out var jwtPayload);
        request.Cookies.TryGetValue("jwtSignature", out var jwtSignature);

        return $"{jwtHeader}.{jwtPayload}.{jwtSignature}";
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