using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using MediatR;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyCreationCommandHandler : IRequestHandler<CompanyCreationCommand, Unit>
{
  private readonly ICompanyLicenseInitializer _companyLicenseInitializer;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;
  private readonly IIdentityGenerator<CompanyId> _identityGenerator;
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public CompanyCreationCommandHandler(
    IIdentityGenerator<CompanyId> identityGenerator,
    ISystemLicenseRetriever systemLicenseRetriever,
    ICompanyLicenseInitializer companyLicenseInitializer,
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _systemLicenseRetriever = systemLicenseRetriever ?? throw new ArgumentNullException(nameof(systemLicenseRetriever));
    _companyLicenseInitializer = companyLicenseInitializer ?? throw new ArgumentNullException(nameof(companyLicenseInitializer));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
  }

  public async Task<Unit> Handle(
    CompanyCreationCommand request,
    CancellationToken cancellationToken)
  {
    var oldEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);
    if (oldEntity.IsSome)
    {
      throw new InvalidOperationException("A company for this tenant has already been created.");
    }

    await ValidateEntitlementsAsync(cancellationToken).ConfigureAwait(false);
    var id = _identityGenerator.Generate();
    CompanyEntity entity = new(id, request.TenantID, request.Name, request.Country);
    await _companyRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    await _companyLicenseInitializer.InitializeAsync(id, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }

  private static bool EnsureMaximumCompanyCountWillNotExceed(
    License<SystemEntitlements> license, int numberOfCompanies)
  {
    return license.Entitlements.MaximumCompanyCount > numberOfCompanies;
  }

  private async Task ValidateEntitlementsAsync(CancellationToken cancellationToken)
  {
    var licenseValidation = await _systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);

    var numberOfCompanies = await _companyQueryRepository.CountAllAsync(cancellationToken).ConfigureAwait(false);

    licenseValidation = licenseValidation
      .Validate(
        license => EnsureMaximumCompanyCountWillNotExceed(license, numberOfCompanies),
        1185884116,
        "The current system license entitlements limit the number of companies that can be created, and this limit has been reached");

    _ = licenseValidation.GetOrThrow();
  }
}
