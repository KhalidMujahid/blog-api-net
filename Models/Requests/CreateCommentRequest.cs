using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record CreateCommentRequest
{
    [Required]
    public string Message { get; init; } = string.Empty;
}
