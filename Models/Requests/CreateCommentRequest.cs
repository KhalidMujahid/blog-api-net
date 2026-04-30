using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record CreateCommentRequest(
    [property: Required] string Message);
