using System.Security.Cryptography;
using System.Text;
using blog_api.Data;
using blog_api.Models;
using blog_api.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace blog_api.Services;

public class AuthService(BlogDbContext dbContext) : IAuthService
{
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var normalizedUsername = request.Username.Trim();
        var exists = await dbContext.Users.AnyAsync(user =>
            user.Username.ToLower() == normalizedUsername.ToLower());

        if (exists)
        {
            return null;
        }

        var user = new AppUser
        {
            Username = normalizedUsername,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var normalizedUsername = request.Username.Trim();
        var user = await dbContext.Users.FirstOrDefaultAsync(existingUser =>
            existingUser.Username.ToLower() == normalizedUsername.ToLower());

        if (user is null || user.PasswordHash != HashPassword(request.Password))
        {
            return null;
        }

        return await BuildAuthResponseAsync(user);
    }

    public Task<AppUser?> GetUserByTokenAsync(string token) =>
        dbContext.AuthTokens
            .AsNoTracking()
            .Where(existingToken => existingToken.Token == token)
            .Select(existingToken => existingToken.User)
            .FirstOrDefaultAsync();

    public Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        var normalizedUsername = username.Trim();
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username.ToLower() == normalizedUsername.ToLower())!;
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(AppUser user)
    {
        var authToken = new AuthToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.AuthTokens.Add(authToken);
        await dbContext.SaveChangesAsync();

        return new AuthResponse(
            authToken.Token,
            new UserProfileResponse(user.Id, user.Username, user.CreatedAt));
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
