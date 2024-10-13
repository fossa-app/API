using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Fossa.API.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using UUIDNext;

namespace Fossa.API.FunctionalTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
  private static readonly Guid _tenantIdNamespaceId = Guid.Parse("a75910c6-2fdb-49fa-a74f-7182055b53f7");
  private static readonly Guid _userIdNamespaceId = Guid.Parse("962dfa41-576b-44f2-8b19-404ee9d865fd");

  public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
      : base(options, logger, encoder) { }

#pragma warning disable MA0051 // Method is too long

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore MA0051 // Method is too long
  {
    if (Context.Request.Headers.Authorization.Count == 0)
    {
      return Task.FromResult(AuthenticateResult.NoResult());
    }

    if (Context.Request.Headers.Authorization.Count != 1)
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("No more than one authorization header should be provided.")));
    }

    var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Context.Request.Headers.Authorization.Single() ?? string.Empty);
    if (!string.Equals(authenticationHeaderValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("Unsupported Schema")));
    }

    var accessToken = authenticationHeaderValue.Parameter ?? string.Empty;
    var accessTokenParts = accessToken.Split('.');

    if (accessTokenParts.Length != 3)
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("Access Token contains invalid number of parts")));
    }

    if (!Ulid.TryParse(accessTokenParts[0], CultureInfo.InvariantCulture, out var _))
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("Access Token first part needs to be ULID")));
    }

    if (!accessTokenParts[1].StartsWith("Tenant", StringComparison.OrdinalIgnoreCase))
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("Access Token second part is tenant, it needs to start with `Tenant`")));
    }

    if (!accessTokenParts[2].StartsWith("User", StringComparison.OrdinalIgnoreCase) && !accessTokenParts[2].StartsWith("ADMIN", StringComparison.OrdinalIgnoreCase))
    {
      return Task.FromResult(AuthenticateResult.Fail(new InvalidOperationException("Access Token third part is user, it needs to start with `User` or `ADMIN`")));
    }

    var tenant = accessTokenParts[1];
    var user = accessTokenParts[2];

    var tenantUserIdNamespaceId = Uuid.NewNameBased(_userIdNamespaceId, tenant);

    var claims = new List<Claim>()
    {
      new(ClaimConstants.TenantId, Uuid.NewNameBased(_tenantIdNamespaceId, tenant).ToString()),
      new(ClaimTypes.Name, $"{tenant} {user}"),
      new(ClaimTypes.NameIdentifier, Uuid.NewNameBased(tenantUserIdNamespaceId, user).ToString()),
    };

    if (user.StartsWith("ADMIN", StringComparison.OrdinalIgnoreCase))
    {
      claims.Add(new Claim(ClaimTypes.Role, Roles.Administrator));
    }

    var identity = new ClaimsIdentity(claims, "Test");
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, "Test");

    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}
