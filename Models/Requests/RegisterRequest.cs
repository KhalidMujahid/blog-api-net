using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record RegisterRequest(
    [property: Required, StringLength(50, MinimumLength = 3)] string Username,
    [property: Required, StringLength(100, MinimumLength = 6)] string Password);
