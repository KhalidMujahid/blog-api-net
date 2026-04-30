using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record LoginRequest(
    [property: Required] string Username,
    [property: Required] string Password);
