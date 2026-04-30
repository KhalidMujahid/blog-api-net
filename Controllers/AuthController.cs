using blog_api.Models.Requests;
using blog_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return result is null
            ? Conflict(new { message = "Username is already taken." })
            : Created("/api/auth/me", result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        return result is null
            ? Unauthorized(new { message = "Invalid username or password." })
            : Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> Me()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }

        var user = await authService.GetUserByUsernameAsync(username);
        return user is null
            ? Unauthorized(new { message = "User is not authenticated." })
            : Ok(new UserProfileResponse(user.Id, user.Username, user.CreatedAt));
    }
}
