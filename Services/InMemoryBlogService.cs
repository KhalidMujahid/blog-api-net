using System.Text.RegularExpressions;
using blog_api.Models;
using blog_api.Models.Requests;

namespace blog_api.Services;

public class InMemoryBlogService : IBlogService
{
    private BlogPost[] _posts;
    private int _nextPostId;
    private int _nextCommentId;
    private readonly IAuthService _authService;

    public InMemoryBlogService(IAuthService authService)
    {
        _authService = authService;
        _posts =
        [
            new BlogPost
            {
                Id = 1,
                AuthorId = 1,
                Title = "Getting Started With ASP.NET Minimal APIs",
                Slug = "getting-started-with-aspnet-minimal-apis",
                Summary = "A quick intro to building small APIs with minimal boilerplate.",
                Content = "Minimal APIs let you ship HTTP endpoints quickly while keeping the code readable.",
                Author = "khalid",
                Tags = ["aspnet", "dotnet", "api"],
                IsPublished = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-8),
                PublishedAt = DateTime.UtcNow.AddDays(-9),
                Comments =
                [
                    new Comment
                    {
                        Id = 1,
                        AuthorId = 1,
                        AuthorName = "Ada",
                        Message = "This was helpful. Looking forward to more posts.",
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    }
                ]
            },
            new BlogPost
            {
                Id = 2,
                AuthorId = 1,
                Title = "Designing a Simple Blog API",
                Slug = "designing-a-simple-blog-api",
                Summary = "Planning routes, validation, and data shape for a blog backend.",
                Content = "A good blog API usually starts with posts, comments, filtering, and clear error responses.",
                Author = "khalid",
                Tags = ["backend", "rest", "blog"],
                IsPublished = false,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                Comments = []
            }
        ];

        _nextPostId = _posts.Max(post => post.Id) + 1;
        _nextCommentId = _posts
            .SelectMany(post => post.Comments)
            .Select(comment => comment.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;
    }

    public BlogPost[] GetPosts(string? search, string? author, string? tag, bool includeDrafts)
    {
        var query = _posts.AsEnumerable();

        if (!includeDrafts)
        {
            query = query.Where(post => post.IsPublished);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(post =>
                post.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                post.Content.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                post.Summary.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(post => post.Author.Equals(author, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(post => post.Tags.Any(existingTag =>
                existingTag.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }

        return query
            .OrderByDescending(post => post.CreatedAt)
            .ToArray();
    }

    public BlogPost? GetPostById(int id) => _posts.FirstOrDefault(post => post.Id == id);

    public BlogPost? GetPostBySlug(string slug) =>
        _posts.FirstOrDefault(post => post.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public bool PostExists(int id) => _posts.Any(post => post.Id == id);

    public BlogPost CreatePost(CreatePostRequest request, string username)
    {
        var user = _authService.GetUserByUsername(username)
            ?? throw new InvalidOperationException("Authenticated user was not found.");
        var now = DateTime.UtcNow;
        var post = new BlogPost
        {
            Id = _nextPostId++,
            AuthorId = user.Id,
            Title = request.Title.Trim(),
            Slug = GenerateUniqueSlug(request.Title),
            Summary = request.Summary?.Trim() ?? string.Empty,
            Content = request.Content.Trim(),
            Author = user.Username,
            Tags = NormalizeTags(request.Tags),
            IsPublished = request.IsPublished,
            CreatedAt = now,
            UpdatedAt = now,
            PublishedAt = request.IsPublished ? now : null,
            Comments = []
        };

        _posts = [.. _posts, post];
        return post;
    }

    public BlogOperationResult<BlogPost> UpdatePost(int id, UpdatePostRequest request, string username)
    {
        var index = Array.FindIndex(_posts, post => post.Id == id);
        if (index < 0)
        {
            return BlogOperationResult<BlogPost>.NotFound;
        }

        var existing = _posts[index];
        if (!existing.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return BlogOperationResult<BlogPost>.Forbidden;
        }

        var normalizedTitle = request.Title.Trim();
        var updated = existing with
        {
            Title = normalizedTitle,
            Slug = existing.Title.Equals(normalizedTitle, StringComparison.Ordinal)
                ? existing.Slug
                : GenerateUniqueSlug(normalizedTitle, existing.Id),
            Summary = request.Summary?.Trim() ?? string.Empty,
            Content = request.Content.Trim(),
            Tags = NormalizeTags(request.Tags),
            IsPublished = request.IsPublished,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = request.IsPublished
                ? existing.PublishedAt ?? DateTime.UtcNow
                : null
        };

        _posts[index] = updated;
        return BlogOperationResult<BlogPost>.Success(updated);
    }

    public BlogOperationResult<BlogPost> SetPublishState(int id, bool isPublished, string username)
    {
        var index = Array.FindIndex(_posts, post => post.Id == id);
        if (index < 0)
        {
            return BlogOperationResult<BlogPost>.NotFound;
        }

        var existing = _posts[index];
        if (!existing.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return BlogOperationResult<BlogPost>.Forbidden;
        }

        var updated = existing with
        {
            IsPublished = isPublished,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = isPublished ? existing.PublishedAt ?? DateTime.UtcNow : null
        };

        _posts[index] = updated;
        return BlogOperationResult<BlogPost>.Success(updated);
    }

    public DeleteStatus DeletePost(int id, string username)
    {
        var post = _posts.FirstOrDefault(existingPost => existingPost.Id == id);
        if (post is null)
        {
            return DeleteStatus.NotFound;
        }

        if (!post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return DeleteStatus.Forbidden;
        }

        _posts = _posts.Where(existingPost => existingPost.Id != id).ToArray();
        return DeleteStatus.Success;
    }

    public Comment[] GetComments(int postId) =>
        _posts.First(post => post.Id == postId).Comments
            .OrderByDescending(comment => comment.CreatedAt)
            .ToArray();

    public Comment? AddComment(int postId, CreateCommentRequest request, string username)
    {
        var index = Array.FindIndex(_posts, post => post.Id == postId);
        if (index < 0)
        {
            return null;
        }

        var user = _authService.GetUserByUsername(username)
            ?? throw new InvalidOperationException("Authenticated user was not found.");
        var comment = new Comment
        {
            Id = _nextCommentId++,
            AuthorId = user.Id,
            AuthorName = user.Username,
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var post = _posts[index];
        _posts[index] = post with
        {
            Comments = [.. post.Comments, comment],
            UpdatedAt = DateTime.UtcNow
        };

        return comment;
    }

    public DeleteStatus DeleteComment(int postId, int commentId, string username)
    {
        var index = Array.FindIndex(_posts, post => post.Id == postId);
        if (index < 0)
        {
            return DeleteStatus.NotFound;
        }

        var post = _posts[index];
        var comment = post.Comments.FirstOrDefault(existingComment => existingComment.Id == commentId);
        if (comment is null)
        {
            return DeleteStatus.NotFound;
        }

        if (!comment.AuthorName.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            !post.Author.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            return DeleteStatus.Forbidden;
        }

        var remainingComments = post.Comments.Where(existingComment => existingComment.Id != commentId).ToArray();

        _posts[index] = post with
        {
            Comments = remainingComments,
            UpdatedAt = DateTime.UtcNow
        };

        return DeleteStatus.Success;
    }

    private static string[] NormalizeTags(string[]? tags) =>
        tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray() ?? [];

    private string GenerateUniqueSlug(string title, int? currentPostId = null)
    {
        var baseSlug = Regex.Replace(title.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "post";
        }

        var slug = baseSlug;
        var suffix = 2;

        while (_posts.Any(post =>
                   post.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase) &&
                   post.Id != currentPostId))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }
}
