using blog_api.Authentication;
using blog_api.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IBlogService, InMemoryBlogService>();
builder.Services.AddSingleton<IAuthService, InMemoryAuthService>();
builder.Services
    .AddAuthentication(TokenAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>(
        TokenAuthenticationDefaults.AuthenticationScheme,
        _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    name = "Blog API",
    version = "1.0.0",
    pattern = "MVC",
    endpoints = new[]
    {
        "/api/auth/register",
        "/api/auth/login",
        "/api/auth/me",
        "/api/posts",
        "/api/posts/{id}",
        "/api/posts/slug/{slug}",
        "/api/posts/{id}/comments"
    }
}));

app.Run();
