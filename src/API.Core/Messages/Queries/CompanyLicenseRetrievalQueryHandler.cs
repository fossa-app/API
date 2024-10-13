using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using LanguageExt.Common;
using MediatR;
using TIKSN.Licensing;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Messages.Queries;

public class CompanyLicenseRetrievalQueryHandler : IRequestHandler<CompanyLicenseRetrievalQuery, License<CompanyEntitlements>>
{
  private readonly ICompanyLicenseRetriever _companyLicenseRetriever;
  private readonly ICompanyQueryRepository _companyQueryRepository;

  public CompanyLicenseRetrievalQueryHandler(
    ICompanyLicenseRetriever companyLicenseRetriever,
    ICompanyQueryRepository companyQueryRepository)
  {
    _companyLicenseRetriever = companyLicenseRetriever ?? throw new ArgumentNullException(nameof(companyLicenseRetriever));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
  }

  public async Task<License<CompanyEntitlements>> Handle(
    CompanyLicenseRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    var companyMaybe = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);

    var licenseValidation = await companyMaybe.MatchAsync(
      company => _companyLicenseRetriever.GetAsync(company.ID, cancellationToken),
      () => Fail<Error, License<CompanyEntitlements>>(Error.New(24711117, "Company is not created yet.")))
      .ConfigureAwait(false);

    return licenseValidation.GetOrThrow();
  }
}
