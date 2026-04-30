namespace blog_api.Models;

public record AppUser
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
