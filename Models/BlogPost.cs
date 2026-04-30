namespace blog_api.Models;

public record BlogPost
{
    public int Id { get; init; }
    public int AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string[] Tags { get; init; } = [];
    public bool IsPublished { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
    public Comment[] Comments { get; init; } = [];
}
