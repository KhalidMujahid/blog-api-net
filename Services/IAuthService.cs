using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AppUser?> GetUserByTokenAsync(string token);
    Task<AppUser?> GetUserByUsernameAsync(string username);
}
