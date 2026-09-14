using airlineRider.Services;
using Microsoft.AspNetCore.Mvc;
using airlineRider.Models.Auth;
using Microsoft.AspNetCore.Authorization;

namespace airlineRider.Controllers;


[ApiController]
[AllowAnonymous]
[Route("/api/user")]
public class UserController(UserService userService): ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> CreateNewUser([FromBody] UserSignupDto dto)
    {
        if (dto.username.IsWhiteSpace())
        {
            return BadRequest("Username already exists.");
        }

        await userService.SaveUser(dto);
        return Ok(new { dto.username, dto.email });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await userService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(response);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await userService.RefreshTokenAsync(request.RefreshToken);
        if (response == null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        return Ok(response);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
    {
        var success = await userService.RevokeRefreshTokenAsync(request.RefreshToken);
        if (!success)
        {
            return BadRequest("Invalid or already revoked refresh token.");
        }

        return Ok("Refresh token revoked.");
    }


}
