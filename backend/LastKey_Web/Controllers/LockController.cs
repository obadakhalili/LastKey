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
                message = $"A lock with the name \"{request.LockName}\" already exists!"
            });

        return createdLock;
    }

    [HttpDelete("{lockId}")]
    public async Task<ActionResult> UnpairLock([FromRoute] int lockId)
    {
        var token = GetToken(Request);
        var userId = GetUserIdFromToken(token);

        var isDeleted = await _lockService.UnpairLockAsync(lockId, userId);

        if (isDeleted == false)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpGet()]
    public async Task<ActionResult<List<Lock>>> RetrieveLocksForUser()
    {
        var token = GetToken(Request);
        var userId = GetUserIdFromToken(token);

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

        var token = GetToken(Request);
        var userId = GetUserIdFromToken(token);

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