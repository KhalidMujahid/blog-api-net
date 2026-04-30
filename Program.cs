using blog_api.Authentication;
using blog_api.Data;
using blog_api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
ConfigurePort(builder);
var connectionString = GetConnectionString(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi("v1");
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DatabaseInitializer>();
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

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

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

static string GetConnectionString(ConfigurationManager configuration)
{
    var databaseUrl = configuration["DATABASE_URL"];
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        return ConvertDatabaseUrlToConnectionString(databaseUrl);
    }

    var configuredConnectionString = configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(configuredConnectionString))
    {
        return configuredConnectionString;
    }

    throw new InvalidOperationException(
        "No PostgreSQL connection string was found. Configure ConnectionStrings:DefaultConnection or DATABASE_URL.");
}

static void ConfigurePort(WebApplicationBuilder builder)
{
    var port = builder.Configuration["PORT"];
    if (!string.IsNullOrWhiteSpace(port))
    {
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }
}

static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':', 2);

    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
        Database = databaseUri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require
    };

    return builder.ConnectionString;
}
