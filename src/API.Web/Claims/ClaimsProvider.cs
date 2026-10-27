using System.Security.Claims;
using Microsoft.Identity.Web;
using TIKSN.Identity;

namespace Fossa.API.Web.Claims;

public class ClaimsProvider :
  ClaimsProviderBase,
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

  protected override Option<ClaimsPrincipal> GetClaimsPrincipal()
  {
    return Optional(_httpContextAccessor.HttpContext?.User);
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
    return GetFound(tenantIdProvider.FindTenantId(), ClaimConstants.TenantId);
  }

  int ITenantIdProvider<int>.GetTenantId()
  {
    ITenantIdProvider<int> tenantIdProvider = this;
    return GetFound(tenantIdProvider.FindTenantId(), ClaimConstants.TenantId);
  }

  Guid IUserIdProvider<Guid>.GetUserId()
  {
    IUserIdProvider<Guid> userIdProvider = this;
    return GetFound(userIdProvider.FindUserId(), ClaimTypes.NameIdentifier);
  }

  int IUserIdProvider<int>.GetUserId()
  {
    IUserIdProvider<int> userIdProvider = this;
    return GetFound(userIdProvider.FindUserId(), ClaimTypes.NameIdentifier);
  }
}
