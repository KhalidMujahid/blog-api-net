using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record PublishPostRequest
{
    [Required]
    public bool IsPublished { get; init; }
}
