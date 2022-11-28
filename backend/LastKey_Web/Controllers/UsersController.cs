using System.Net;
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
    public async Task<ActionResult<User>> CreateUser([FromForm] CreateUserRequest request)
    {
        if (await _userService.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new {
                message = "Username already exists"
            });
        }
        
        var createdUser = await _userService.CreateUserAsync(request);

        return Ok(createdUser);
    }
    

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser([FromBody] LoginUserRequest request)
    {
        var authenticationResponse = await _userService.AuthenticateUserAsync(request);

        if (authenticationResponse == default)
            return Unauthorized();

        HttpContext.Response.Cookies.Append("jwtHeader", authenticationResponse.JwtHeader!, new CookieOptions
        {
            HttpOnly = true
        });
        
        HttpContext.Response.Cookies.Append("jwtSignature", authenticationResponse.JwtSignature!, new CookieOptions
        {
            HttpOnly = true
        });
        
        HttpContext.Response.Cookies.Append("jwtPayload", authenticationResponse.JwtPayload!, new CookieOptions
        {
            HttpOnly = false
        });

        return Ok(authenticationResponse.User);
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