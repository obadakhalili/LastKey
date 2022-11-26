using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Controller]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        var createdUser = await _userService.CreateUserAsync(request);

        return Ok(createdUser);
    }
    

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser([FromBody] LoginUserRequest request)
    {
        var user = await _userService.AuthenticateUserAsync(request);

        if (user == default)
            return Unauthorized();

        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult LogoutUser()
    {
        _userService.ClearCookies();

        return Ok();
    }
    
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<User>> GetUserInfo(int userId)
    {
        var userInfo = await _userService.RetrieveUserInfoByIdAsync(userId);

        if (userInfo == null)
            return NotFound();

        return Ok(userInfo);
    }
}