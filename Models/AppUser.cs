namespace blog_api.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ICollection<AuthToken> Tokens { get; set; } = [];
    public ICollection<BlogPost> Posts { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}
