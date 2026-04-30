using blog_api.Models;
using blog_api.Models.Requests;
using blog_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blog_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(IBlogService blogService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<BlogPost>> GetPosts(
        [FromQuery] string? search,
        [FromQuery] string? author,
        [FromQuery] string? tag,
        [FromQuery] bool includeDrafts = false)
    {
        var posts = blogService.GetPosts(search, author, tag, includeDrafts);
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public ActionResult<BlogPost> GetPostById(int id)
    {
        var post = blogService.GetPostById(id);
        return post is null ? NotFound(new { message = "Post not found." }) : Ok(post);
    }

    [HttpGet("slug/{slug}")]
    public ActionResult<BlogPost> GetPostBySlug(string slug)
    {
        var post = blogService.GetPostBySlug(slug);
        return post is null ? NotFound(new { message = "Post not found." }) : Ok(post);
    }

    [Authorize]
    [HttpPost]
    public ActionResult<BlogPost> CreatePost([FromBody] CreatePostRequest request)
    {
        var username = User.Identity?.Name!;
        var post = blogService.CreatePost(request, username);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public ActionResult<BlogPost> UpdatePost(int id, [FromBody] UpdatePostRequest request)
    {
        var username = User.Identity?.Name!;
        var result = blogService.UpdatePost(id, request, username);
        return result.Status switch
        {
            OperationStatus.NotFound => NotFound(new { message = "Post not found." }),
            OperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Value)
        };
    }

    [Authorize]
    [HttpPatch("{id:int}/publish")]
    public ActionResult<BlogPost> SetPublishState(int id, [FromBody] PublishPostRequest request)
    {
        var username = User.Identity?.Name!;
        var result = blogService.SetPublishState(id, request.IsPublished, username);
        return result.Status switch
        {
            OperationStatus.NotFound => NotFound(new { message = "Post not found." }),
            OperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Value)
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult DeletePost(int id)
    {
        var username = User.Identity?.Name!;
        var result = blogService.DeletePost(id, username);
        return result switch
        {
            DeleteStatus.NotFound => NotFound(new { message = "Post not found." }),
            DeleteStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [HttpGet("{id:int}/comments")]
    public ActionResult<IEnumerable<Comment>> GetComments(int id)
    {
        if (!blogService.PostExists(id))
        {
            return NotFound(new { message = "Post not found." });
        }

        return Ok(blogService.GetComments(id));
    }

    [Authorize]
    [HttpPost("{id:int}/comments")]
    public ActionResult<Comment> AddComment(int id, [FromBody] CreateCommentRequest request)
    {
        var username = User.Identity?.Name!;
        var comment = blogService.AddComment(id, request, username);
        return comment is null
            ? NotFound(new { message = "Post not found." })
            : Created($"/api/posts/{id}/comments/{comment.Id}", comment);
    }

    [Authorize]
    [HttpDelete("{postId:int}/comments/{commentId:int}")]
    public IActionResult DeleteComment(int postId, int commentId)
    {
        var username = User.Identity?.Name!;
        var result = blogService.DeleteComment(postId, commentId, username);
        return result switch
        {
            DeleteStatus.NotFound => NotFound(new { message = "Post or comment not found." }),
            DeleteStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }
}
