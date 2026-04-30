using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public interface IAuthService
{
    AuthResponse? Register(RegisterRequest request);
    AuthResponse? Login(LoginRequest request);
    AppUser? GetUserByToken(string token);
    AppUser? GetUserByUsername(string username);
}
