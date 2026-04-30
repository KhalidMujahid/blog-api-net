using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;
}
