using System.Text.Json.Serialization;

namespace blog_api.Models;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public BlogPost? Post { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }
}
