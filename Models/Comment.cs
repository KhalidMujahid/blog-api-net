namespace blog_api.Models;

public record Comment
{
    public int Id { get; init; }
    public int AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
