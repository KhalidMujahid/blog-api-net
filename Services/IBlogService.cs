using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public interface IBlogService
{
    Task<BlogPost[]> GetPostsAsync(string? search, string? author, string? tag, bool includeDrafts);
    Task<BlogPost?> GetPostByIdAsync(int id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<bool> PostExistsAsync(int id);
    Task<BlogPost> CreatePostAsync(CreatePostRequest request, string username);
    Task<BlogOperationResult<BlogPost>> UpdatePostAsync(int id, UpdatePostRequest request, string username);
    Task<BlogOperationResult<BlogPost>> SetPublishStateAsync(int id, bool isPublished, string username);
    Task<DeleteStatus> DeletePostAsync(int id, string username);
    Task<Comment[]> GetCommentsAsync(int postId);
    Task<Comment?> AddCommentAsync(int postId, CreateCommentRequest request, string username);
    Task<DeleteStatus> DeleteCommentAsync(int postId, int commentId, string username);
}
