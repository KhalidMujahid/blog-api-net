using System.Security.Claims;
using System.Text.Encodings.Web;
using blog_api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace blog_api.Authentication;

public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAuthService _authService;

    public TokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IAuthService authService)
        : base(options, logger, encoder)
    {
        _authService = authService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var headerValue = authorizationHeader.ToString();
        if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = headerValue["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing token."));
        }

        var user = _authService.GetUserByToken(token);
        if (user is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var identity = new ClaimsIdentity(claims, TokenAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TokenAuthenticationDefaults.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
