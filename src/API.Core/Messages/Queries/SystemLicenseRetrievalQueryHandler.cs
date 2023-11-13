using Fossa.API.Core.Extensions;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using MediatR;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Queries;

public class
  SystemLicenseRetrievalQueryHandler : IRequestHandler<SystemLicenseRetrievalQuery, License<SystemEntitlements>>
{
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public SystemLicenseRetrievalQueryHandler(ISystemLicenseRetriever systemLicenseRetriever)
  {
    _systemLicenseRetriever = systemLicenseRetriever ?? throw new ArgumentNullException(nameof(systemLicenseRetriever));
  }

  public async Task<License<SystemEntitlements>> Handle(SystemLicenseRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    var licenseValidation = await _systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);

    return licenseValidation.GetOrThrow();
  }
}
