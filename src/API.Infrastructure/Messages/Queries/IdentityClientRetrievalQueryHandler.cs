using Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;
using Fossa.API.Infrastructure.Models;
using Fossa.API.Infrastructure.RestClients;
using TIKSN.Data;

namespace Fossa.API.Infrastructure.Messages.Queries;

public partial class IdentityClientRetrievalQueryHandler : IRequestHandler<IdentityClientRetrievalQuery, IdentityClient>
{
  private readonly Lst<IFusionAuthApplicationFilter> _fusionAuthApplicationFilters;
  private readonly IFusionAuthRestClient _fusionAuthRestClient;

  public IdentityClientRetrievalQueryHandler(
    IFusionAuthRestClient fusionAuthRestClient,
    ActiveFusionAuthApplicationFilter activeFusionAuthApplicationFilter,
    OriginFusionAuthApplicationFilter originFusionAuthApplicationFilter)
  {
    _fusionAuthRestClient = fusionAuthRestClient ?? throw new ArgumentNullException(nameof(fusionAuthRestClient));
    _fusionAuthApplicationFilters = List<IFusionAuthApplicationFilter>(
      activeFusionAuthApplicationFilter,
      originFusionAuthApplicationFilter);
  }

  public async Task<IdentityClient> Handle(
    IdentityClientRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    var applicationsListingResponse = await _fusionAuthRestClient.GetApplicationsAsync(cancellationToken).ConfigureAwait(false);

    var applications = applicationsListingResponse?.Applications.ToSeq() ?? [];

    foreach (var fusionAuthApplicationFilter in _fusionAuthApplicationFilters)
    {
      applications = await fusionAuthApplicationFilter.FilterAsync(applications, request, cancellationToken).ConfigureAwait(false);
    }

    var application = applications.Match(Empty: () => throw new EntityNotFoundException(), One: x => x, More: (_, _) => throw new EntityNotFoundException());

    return new IdentityClient(application.Id, application.Name ?? string.Empty, application.TenantId);
  }
}
