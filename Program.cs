using blog_api.Authentication;
using blog_api.Services;
using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi("v1");
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

app.MapOpenApi("/openapi/{documentName}.json");
app.MapScalarApiReference("/scalar", options =>
{
    options.Title = "Blog API";
});
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    name = "Blog API",
    version = "1.0.0",
    pattern = "MVC",
    endpoints = new[]
    {
        "/scalar",
        "/openapi/v1.json",
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
