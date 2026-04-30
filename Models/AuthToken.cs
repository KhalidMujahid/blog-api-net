using System.Text.Json.Serialization;

namespace blog_api.Models;

public class AuthToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }
}
