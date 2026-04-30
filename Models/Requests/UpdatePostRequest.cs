using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record UpdatePostRequest
{
    [Required]
    [StringLength(150)]
    public string Title { get; init; } = string.Empty;

    public string? Summary { get; init; }

    [Required]
    public string Content { get; init; } = string.Empty;

    public string[]? Tags { get; init; }

    public bool IsPublished { get; init; }
}
