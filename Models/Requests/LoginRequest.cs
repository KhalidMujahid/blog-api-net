using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
