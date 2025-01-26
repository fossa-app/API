using Fossa.API.Infrastructure.Models;

namespace Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;

public class OriginFusionAuthApplicationFilter : IFusionAuthApplicationFilter
{
  public Task<Seq<FusionAuthApplication>> FilterAsync(
    Seq<FusionAuthApplication> fusionAuthApplications,
    IdentityClientRetrievalQuery query,
    CancellationToken cancellationToken)
  {
    return Task.FromResult(
      query.Origin.Match(
        origin => fusionAuthApplications.Filter(x => HasOrigin(x, origin)),
        fusionAuthApplications));
  }

  private static bool HasOrigin(
    string[] authorizedOriginUrLs,
    Uri origin)
  {
    return authorizedOriginUrLs
      .Map(x => new Uri(x))
      .Any(x => x == origin);
  }

  private static bool HasOrigin(
    FusionAuthApplication application,
    Uri origin)
  {
    return Optional(application.OauthConfiguration?.AuthorizedOriginUrLs)
      .Match(
        authorizedOriginUrLs => HasOrigin(authorizedOriginUrLs, origin),
        None: false);
  }
}
