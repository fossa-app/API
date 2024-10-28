using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using LanguageExt.Common;
using MediatR;
using TIKSN.Licensing;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyLicenseCreationCommandHandler : IRequestHandler<CompanyLicenseCreationCommand, Unit>
{
  private readonly ICompanyLicenseCreator _companyLicenseCreator;
  private readonly ICompanyQueryRepository _companyQueryRepository;

  public CompanyLicenseCreationCommandHandler(
    ICompanyLicenseCreator companyLicenseCreator,
    ICompanyQueryRepository companyQueryRepository)
  {
    _companyLicenseCreator = companyLicenseCreator ?? throw new ArgumentNullException(nameof(companyLicenseCreator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
  }

  public async Task<Unit> Handle(CompanyLicenseCreationCommand request, CancellationToken cancellationToken)
  {
    var companyMaybe = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);

    var licenseValidation = await companyMaybe.MatchAsync(
      company => _companyLicenseCreator.CreateAsync(company.ID, request.LicenseData, cancellationToken),
      () => Fail<Error, License<CompanyEntitlements>>(Error.New(24711117, "Company is not created yet.")))
      .ConfigureAwait(false);

    _ = licenseValidation.GetOrThrow();

    return Unit.Value;
  }
}
