using System.Security.Cryptography;
using System.Text;
using blog_api.Models;
using Microsoft.EntityFrameworkCore;

namespace blog_api.Data;

public class DatabaseInitializer(BlogDbContext dbContext)
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.MigrateAsync();

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var user = new AppUser
        {
            Username = "khalid",
            PasswordHash = HashPassword("password123"),
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };

        var firstPost = new BlogPost
        {
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
            User = user
        };

        var secondPost = new BlogPost
        {
            Title = "Designing a Simple Blog API",
            Slug = "designing-a-simple-blog-api",
            Summary = "Planning routes, validation, and data shape for a blog backend.",
            Content = "A good blog API usually starts with posts, comments, filtering, and clear error responses.",
            Author = "khalid",
            Tags = ["backend", "rest", "blog"],
            IsPublished = false,
            CreatedAt = DateTime.UtcNow.AddDays(-4),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            User = user
        };

        var comment = new Comment
        {
            AuthorName = "khalid",
            Message = "This was helpful. Looking forward to more posts.",
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            User = user,
            Post = firstPost
        };

        dbContext.Users.Add(user);
        dbContext.Posts.AddRange(firstPost, secondPost);
        dbContext.Comments.Add(comment);

        await dbContext.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
