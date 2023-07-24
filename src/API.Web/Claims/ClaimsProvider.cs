using System.Security.Claims;
using Fossa.API.Core.User;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Fossa.API.Web.Claims;

public class ClaimsProvider : IUserIdProvider<Guid>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ClaimsProvider(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
  }

  public Option<Guid> FindUserId()
  {
    return FindFirstClaimValue(ClaimTypes.NameIdentifier, Guid.Parse);
  }

  public Guid GetUserId()
  {
    return FindUserId().IfNone(() => throw new ClaimNotFoundException());
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
}
