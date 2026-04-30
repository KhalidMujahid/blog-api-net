using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public interface IBlogService
{
    BlogPost[] GetPosts(string? search, string? author, string? tag, bool includeDrafts);
    BlogPost? GetPostById(int id);
    BlogPost? GetPostBySlug(string slug);
    bool PostExists(int id);
    BlogPost CreatePost(CreatePostRequest request, string username);
    BlogOperationResult<BlogPost> UpdatePost(int id, UpdatePostRequest request, string username);
    BlogOperationResult<BlogPost> SetPublishState(int id, bool isPublished, string username);
    DeleteStatus DeletePost(int id, string username);
    Comment[] GetComments(int postId);
    Comment? AddComment(int postId, CreateCommentRequest request, string username);
    DeleteStatus DeleteComment(int postId, int commentId, string username);
}
