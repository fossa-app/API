using Fossa.API.Infrastructure.Models;

namespace Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;

public interface IFusionAuthApplicationFilter
{
  Task<Seq<FusionAuthApplication>> FilterAsync(
    Seq<FusionAuthApplication> fusionAuthApplications,
    IdentityClientRetrievalQuery query,
    CancellationToken cancellationToken);
}
