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
    public async Task<ActionResult<IEnumerable<BlogPost>>> GetPosts(
        [FromQuery] string? search,
        [FromQuery] string? author,
        [FromQuery] string? tag,
        [FromQuery] bool includeDrafts = false)
    {
        var posts = await blogService.GetPostsAsync(search, author, tag, includeDrafts);
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogPost>> GetPostById(int id)
    {
        var post = await blogService.GetPostByIdAsync(id);
        return post is null ? NotFound(new { message = "Post not found." }) : Ok(post);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<BlogPost>> GetPostBySlug(string slug)
    {
        var post = await blogService.GetPostBySlugAsync(slug);
        return post is null ? NotFound(new { message = "Post not found." }) : Ok(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BlogPost>> CreatePost([FromBody] CreatePostRequest request)
    {
        var username = User.Identity?.Name!;
        var post = await blogService.CreatePostAsync(request, username);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BlogPost>> UpdatePost(int id, [FromBody] UpdatePostRequest request)
    {
        var username = User.Identity?.Name!;
        var result = await blogService.UpdatePostAsync(id, request, username);
        return result.Status switch
        {
            OperationStatus.NotFound => NotFound(new { message = "Post not found." }),
            OperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Value)
        };
    }

    [Authorize]
    [HttpPatch("{id:int}/publish")]
    public async Task<ActionResult<BlogPost>> SetPublishState(int id, [FromBody] PublishPostRequest request)
    {
        var username = User.Identity?.Name!;
        var result = await blogService.SetPublishStateAsync(id, request.IsPublished, username);
        return result.Status switch
        {
            OperationStatus.NotFound => NotFound(new { message = "Post not found." }),
            OperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Value)
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var username = User.Identity?.Name!;
        var result = await blogService.DeletePostAsync(id, username);
        return result switch
        {
            DeleteStatus.NotFound => NotFound(new { message = "Post not found." }),
            DeleteStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int id)
    {
        if (!await blogService.PostExistsAsync(id))
        {
            return NotFound(new { message = "Post not found." });
        }

        return Ok(await blogService.GetCommentsAsync(id));
    }

    [Authorize]
    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<Comment>> AddComment(int id, [FromBody] CreateCommentRequest request)
    {
        var username = User.Identity?.Name!;
        var comment = await blogService.AddCommentAsync(id, request, username);
        return comment is null
            ? NotFound(new { message = "Post not found." })
            : Created($"/api/posts/{id}/comments/{comment.Id}", comment);
    }

    [Authorize]
    [HttpDelete("{postId:int}/comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int postId, int commentId)
    {
        var username = User.Identity?.Name!;
        var result = await blogService.DeleteCommentAsync(postId, commentId, username);
        return result switch
        {
            DeleteStatus.NotFound => NotFound(new { message = "Post or comment not found." }),
            DeleteStatus.Forbidden => Forbid(),
            _ => NoContent()
        };
    }
}
