using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyLicenseCreationCommandHandler : IRequestHandler<CompanyLicenseCreationCommand, Unit>
{
  private readonly ICompanyLicenseCreator _companyLicenseCreator;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IPublisher _publisher;

  public CompanyLicenseCreationCommandHandler(
    ICompanyLicenseCreator companyLicenseCreator,
    ICompanyQueryRepository companyQueryRepository,
    IPublisher publisher)
  {
    _companyLicenseCreator = companyLicenseCreator ?? throw new ArgumentNullException(nameof(companyLicenseCreator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(CompanyLicenseCreationCommand request, CancellationToken cancellationToken)
  {
    var companyMaybe = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);

    var licenseValidation = await companyMaybe.MatchAsync(
      company => _companyLicenseCreator.CreateAsync(company.ID, request.LicenseData, cancellationToken),
      () => Fail<Error, License<CompanyEntitlements>>(Error.New(24711117, "Company is not created yet.")))
      .ConfigureAwait(false);

    var license = licenseValidation.GetOrThrow();

    await companyMaybe.IfSomeAsync(async company =>
    {
      var licenseCreatedEvent = new CompanyLicenseCreatedEvent(
        request.TenantID,
        company.ID,
        license);

      await _publisher.Publish(licenseCreatedEvent, cancellationToken).ConfigureAwait(false);
    }).ConfigureAwait(false);

    return Unit.Value;
  }
}
