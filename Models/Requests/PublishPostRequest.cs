using System.ComponentModel.DataAnnotations;

namespace blog_api.Models.Requests;

public record PublishPostRequest([property: Required] bool IsPublished);
