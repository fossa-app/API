using Fossa.API.Infrastructure.Models;

namespace Fossa.API.Infrastructure.RestClients;

public interface IFusionAuthRestClient
{
  Task<FusionAuthApplicationsListingResponse?> GetApplicationsAsync(CancellationToken cancellationToken);
}
