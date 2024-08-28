using Fossa.API.Infrastructure.Models;
using LanguageExt;

namespace Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;

public class ActiveFusionAuthApplicationFilter : IFusionAuthApplicationFilter
{
  public Task<Seq<FusionAuthApplication>> FilterAsync(
    Seq<FusionAuthApplication> fusionAuthApplications,
    IdentityClientRetrievalQuery query,
    CancellationToken cancellationToken)
  {
    return Task.FromResult(fusionAuthApplications.Filter(x => x.Active));
  }
}
