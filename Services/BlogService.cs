using System.Text.RegularExpressions;
using blog_api.Data;
using blog_api.Models;
using blog_api.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace blog_api.Services;

public class BlogService(BlogDbContext dbContext, IAuthService authService) : IBlogService
{
    public async Task<BlogPost[]> GetPostsAsync(string? search, string? author, string? tag, bool includeDrafts)
    {
        var query = BuildPostQuery();

        if (!includeDrafts)
        {
            query = query.Where(post => post.IsPublished);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(post =>
                EF.Functions.ILike(post.Title, $"%{search}%") ||
                EF.Functions.ILike(post.Content, $"%{search}%") ||
                EF.Functions.ILike(post.Summary, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(post => EF.Functions.ILike(post.Author, author));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(post => post.Tags.Contains(tag.ToLower()));
        }

        return await query
            .OrderByDescending(post => post.CreatedAt)
            .ToArrayAsync();
    }

    public Task<BlogPost?> GetPostByIdAsync(int id) =>
        BuildPostQuery().FirstOrDefaultAsync(post => post.Id == id);

    public Task<BlogPost?> GetPostBySlugAsync(string slug) =>
        BuildPostQuery().FirstOrDefaultAsync(post => post.Slug == slug);

    public Task<bool> PostExistsAsync(int id) =>
        dbContext.Posts.AsNoTracking().AnyAsync(post => post.Id == id);

    public async Task<BlogPost> CreatePostAsync(CreatePostRequest request, string username)
    {
        var user = await authService.GetUserByUsernameAsync(username)
            ?? throw new InvalidOperationException("Authenticated user was not found.");

        var now = DateTime.UtcNow;
        var post = new BlogPost
        {
            AuthorId = user.Id,
            Title = request.Title.Trim(),
            Slug = await GenerateUniqueSlugAsync(request.Title),
            Summary = request.Summary?.Trim() ?? string.Empty,
            Content = request.Content.Trim(),
            Author = user.Username,
            Tags = NormalizeTags(request.Tags),
            IsPublished = request.IsPublished,
            CreatedAt = now,
            UpdatedAt = now,
            PublishedAt = request.IsPublished ? now : null
        };

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync();

        return post;
    }

    public async Task<BlogOperationResult<BlogPost>> UpdatePostAsync(int id, UpdatePostRequest request, string username)
    {
        var post = await dbContext.Posts
            .Include(existingPost => existingPost.Comments)
            .FirstOrDefaultAsync(existingPost => existingPost.Id == id);

        if (post is null)
        {
            return BlogOperationResult<BlogPost>.NotFound;
        }

        if (!post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return BlogOperationResult<BlogPost>.Forbidden;
        }

        var normalizedTitle = request.Title.Trim();
        post.Title = normalizedTitle;
        post.Slug = await GenerateUniqueSlugAsync(normalizedTitle, post.Id);
        post.Summary = request.Summary?.Trim() ?? string.Empty;
        post.Content = request.Content.Trim();
        post.Tags = NormalizeTags(request.Tags);
        post.IsPublished = request.IsPublished;
        post.UpdatedAt = DateTime.UtcNow;
        post.PublishedAt = request.IsPublished
            ? post.PublishedAt ?? DateTime.UtcNow
            : null;

        await dbContext.SaveChangesAsync();
        return BlogOperationResult<BlogPost>.Success(post);
    }

    public async Task<BlogOperationResult<BlogPost>> SetPublishStateAsync(int id, bool isPublished, string username)
    {
        var post = await dbContext.Posts
            .Include(existingPost => existingPost.Comments)
            .FirstOrDefaultAsync(existingPost => existingPost.Id == id);

        if (post is null)
        {
            return BlogOperationResult<BlogPost>.NotFound;
        }

        if (!post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return BlogOperationResult<BlogPost>.Forbidden;
        }

        post.IsPublished = isPublished;
        post.UpdatedAt = DateTime.UtcNow;
        post.PublishedAt = isPublished ? post.PublishedAt ?? DateTime.UtcNow : null;

        await dbContext.SaveChangesAsync();
        return BlogOperationResult<BlogPost>.Success(post);
    }

    public async Task<DeleteStatus> DeletePostAsync(int id, string username)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(existingPost => existingPost.Id == id);
        if (post is null)
        {
            return DeleteStatus.NotFound;
        }

        if (!post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return DeleteStatus.Forbidden;
        }

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
        return DeleteStatus.Success;
    }

    public async Task<Comment[]> GetCommentsAsync(int postId) =>
        await dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderByDescending(comment => comment.CreatedAt)
            .ToArrayAsync();

    public async Task<Comment?> AddCommentAsync(int postId, CreateCommentRequest request, string username)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(existingPost => existingPost.Id == postId);
        if (post is null)
        {
            return null;
        }

        var user = await authService.GetUserByUsernameAsync(username)
            ?? throw new InvalidOperationException("Authenticated user was not found.");

        var comment = new Comment
        {
            PostId = postId,
            AuthorId = user.Id,
            AuthorName = user.Username,
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Comments.Add(comment);
        post.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return comment;
    }

    public async Task<DeleteStatus> DeleteCommentAsync(int postId, int commentId, string username)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(existingPost => existingPost.Id == postId);
        if (post is null)
        {
            return DeleteStatus.NotFound;
        }

        var comment = await dbContext.Comments.FirstOrDefaultAsync(existingComment =>
            existingComment.PostId == postId && existingComment.Id == commentId);
        if (comment is null)
        {
            return DeleteStatus.NotFound;
        }

        if (!comment.AuthorName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            !post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return DeleteStatus.Forbidden;
        }

        dbContext.Comments.Remove(comment);
        post.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return DeleteStatus.Success;
    }

    private IQueryable<BlogPost> BuildPostQuery() =>
        dbContext.Posts
            .AsNoTracking()
            .Include(post => post.Comments.OrderByDescending(comment => comment.CreatedAt));

    private static string[] NormalizeTags(string[]? tags) =>
        tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray() ?? [];

    private async Task<string> GenerateUniqueSlugAsync(string title, int? currentPostId = null)
    {
        var baseSlug = Regex.Replace(title.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "post";
        }

        var slug = baseSlug;
        var suffix = 2;

        while (await dbContext.Posts.AnyAsync(post =>
                   post.Slug == slug &&
                   post.Id != currentPostId))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }
}
