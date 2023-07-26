using System.Security.Claims;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using LanguageExt;
using Microsoft.Identity.Web;
using static LanguageExt.Prelude;

namespace Fossa.API.Web.Claims;

public class ClaimsProvider :
  IUserIdProvider<Guid>,
  IUserIdProvider<int>,
  ITenantIdProvider<Guid>,
  ITenantIdProvider<int>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ClaimsProvider(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
  }

  Option<Guid> ITenantIdProvider<Guid>.FindTenantId()
  {
    return FindFirstClaimValue(ClaimConstants.TenantId, Guid.Parse);
  }

  Option<int> ITenantIdProvider<int>.FindTenantId()
  {
    return FindFirstClaimValue(ClaimConstants.TenantId, int.Parse);
  }

  Option<Guid> IUserIdProvider<Guid>.FindUserId()
  {
    return FindFirstClaimValue(ClaimTypes.NameIdentifier, Guid.Parse);
  }

  Option<int> IUserIdProvider<int>.FindUserId()
  {
    return FindFirstClaimValue(ClaimTypes.NameIdentifier, int.Parse);
  }

  Guid ITenantIdProvider<Guid>.GetTenantId()
  {
    ITenantIdProvider<Guid> tenantIdProvider = this;
    return GetFound(tenantIdProvider.FindTenantId());
  }

  int ITenantIdProvider<int>.GetTenantId()
  {
    ITenantIdProvider<int> tenantIdProvider = this;
    return GetFound(tenantIdProvider.FindTenantId());
  }

  Guid IUserIdProvider<Guid>.GetUserId()
  {
    IUserIdProvider<Guid> userIdProvider = this;
    return GetFound(userIdProvider.FindUserId());
  }

  int IUserIdProvider<int>.GetUserId()
  {
    IUserIdProvider<int> userIdProvider = this;
    return GetFound(userIdProvider.FindUserId());
  }

  private Option<T> FindFirstClaimValue<T>(
    string type,
    Func<string, T> parser)
  {
    if (parser is null)
    {
      throw new ArgumentNullException(nameof(parser));
    }

    var rawValue = FindFirstClaimValue(type);

    return rawValue.Map(parser);
  }

  private Option<string> FindFirstClaimValue(string type)
  {
    var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(type);

    return Optional(claim).Map(x => x.Value);
  }

  private T GetFound<T>(Option<T> found)
  {
    return found.IfNone(() => throw new ClaimNotFoundException());
  }
}
