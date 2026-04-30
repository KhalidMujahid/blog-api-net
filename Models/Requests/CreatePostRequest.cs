using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record CreatePostRequest(
    [property: Required, StringLength(150)] string Title,
    string? Summary,
    [property: Required] string Content,
    string[]? Tags,
    bool IsPublished);
