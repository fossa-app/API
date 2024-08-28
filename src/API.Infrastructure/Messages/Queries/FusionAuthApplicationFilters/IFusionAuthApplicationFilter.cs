using Fossa.API.Infrastructure.Models;
using LanguageExt;

namespace Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;

public interface IFusionAuthApplicationFilter
{
  Task<Seq<FusionAuthApplication>> FilterAsync(
    Seq<FusionAuthApplication> fusionAuthApplications,
    IdentityClientRetrievalQuery query,
    CancellationToken cancellationToken);
}
