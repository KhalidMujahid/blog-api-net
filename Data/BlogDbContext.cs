using blog_api.Models;
using Microsoft.EntityFrameworkCore;

namespace blog_api.Data;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AuthToken> AuthTokens => Set<AuthToken>();
    public DbSet<BlogPost> Posts => Set<BlogPost>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Username).IsUnique();
            entity.Property(user => user.Username).HasMaxLength(50);
            entity.Property(user => user.PasswordHash).HasMaxLength(256);
            entity.HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .HasForeignKey(post => post.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(user => user.Comments)
                .WithOne(comment => comment.User)
                .HasForeignKey(comment => comment.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(user => user.Tokens)
                .WithOne(token => token.User)
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuthToken>(entity =>
        {
            entity.ToTable("auth_tokens");
            entity.HasKey(token => token.Id);
            entity.HasIndex(token => token.Token).IsUnique();
            entity.Property(token => token.Token).HasMaxLength(128);
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.ToTable("posts");
            entity.HasKey(post => post.Id);
            entity.HasIndex(post => post.Slug).IsUnique();
            entity.Property(post => post.Title).HasMaxLength(150);
            entity.Property(post => post.Slug).HasMaxLength(180);
            entity.Property(post => post.Summary).HasMaxLength(500);
            entity.Property(post => post.Author).HasMaxLength(50);
            entity.Property(post => post.Tags).HasColumnType("text[]");
            entity.HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.HasKey(comment => comment.Id);
            entity.Property(comment => comment.AuthorName).HasMaxLength(50);
            entity.Property(comment => comment.Message).HasMaxLength(2000);
        });
    }
}
