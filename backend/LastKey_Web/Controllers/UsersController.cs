using LastKey_Domain.Entities.DTOs;
using LastKey_Domain.Interfaces;
using LastKey_Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastKey_Web.Controllers;

[Authorize]
[Controller]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [AllowAnonymous]
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

        createdUser.UserImage = null;

        return Ok(createdUser);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpPost("members")]
    public async Task<ActionResult<User>> AddMembers([FromForm] CreateUserRequest request)
    {
        if (await _userService.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new {
                message = "Username already exists"
            });
        }
        
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var createdUser = await _userService.AddMemberToUserAsync(userId, request);

        createdUser.UserImage = null;
        
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

        authenticationResponse.User!.UserImage = null;
        
        return Ok(authenticationResponse.User);
    }

    [HttpPost("logout")]
    public ActionResult LogoutUser()
    {
        _userService.ClearCookies();

        return Ok();
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<User>> GetUserInfo()
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var userInfo = await _userService.RetrieveUserInfoByIdAsync(userId);

        if (userInfo == null)
            return NotFound();

        userInfo.UserImage = null;

        return Ok(userInfo);
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpGet("members")]
    public ActionResult<List<User>> GetMembersForUser()
    {
        var userId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var members = _userService.RetrieveMembersForUserAsync(userId);

        return Ok(members != null ? members.Select(member => new User
        {
            UserId = member.UserId,
            Username = member.Username,
            FullName = member.FullName,
            IsAdmin = member.IsAdmin,
            AdminId = member.AdminId,
            UserImage = null
        }) : new List<User>());
    }

    [Authorize(Roles = nameof(Roles.Admin))]
    [HttpDelete("members/{memberId}")]
    public async Task<ActionResult> DeleteMember(int memberId)
    {
        var adminId = JwtSecurityHelper.GetUserIdFromToken(Request);

        var isDeleted = await _userService.RemoveMemberAsync(memberId, adminId);

        if (!isDeleted)
        {
            return NotFound(new
            {
                message = "The specified user was not found!"
            });
        }

        return Ok();
    }
}