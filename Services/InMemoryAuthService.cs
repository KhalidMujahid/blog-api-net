using System.Security.Cryptography;
using System.Text;
using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public class InMemoryAuthService : IAuthService
{
    private AppUser[] _users;
    private Dictionary<string, int> _tokens;
    private int _nextUserId;

    public InMemoryAuthService()
    {
        _users =
        [
            new AppUser
            {
                Id = 1,
                Username = "khalid",
                PasswordHash = HashPassword("password123"),
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        ];
        _tokens = new Dictionary<string, int>(StringComparer.Ordinal);
        _nextUserId = _users.Max(user => user.Id) + 1;
    }

    public AuthResponse? Register(RegisterRequest request)
    {
        var normalizedUsername = request.Username.Trim();
        if (_users.Any(user => user.Username.Equals(normalizedUsername, StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        var user = new AppUser
        {
            Id = _nextUserId++,
            Username = normalizedUsername,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _users = [.. _users, user];
        return BuildAuthResponse(user);
    }

    public AuthResponse? Login(LoginRequest request)
    {
        var user = _users.FirstOrDefault(existingUser =>
            existingUser.Username.Equals(request.Username.Trim(), StringComparison.OrdinalIgnoreCase));

        if (user is null || user.PasswordHash != HashPassword(request.Password))
        {
            return null;
        }

        return BuildAuthResponse(user);
    }

    public AppUser? GetUserByToken(string token)
    {
        if (!_tokens.TryGetValue(token, out var userId))
        {
            return null;
        }

        return _users.FirstOrDefault(user => user.Id == userId);
    }

    public AppUser? GetUserByUsername(string username) =>
        _users.FirstOrDefault(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    private AuthResponse BuildAuthResponse(AppUser user)
    {
        var token = Guid.NewGuid().ToString("N");
        _tokens[token] = user.Id;
        return new AuthResponse(token, new UserProfileResponse(user.Id, user.Username, user.CreatedAt));
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
