using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using TIKSN.Data;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeCreationCommandHandler : IRequestHandler<EmployeeCreationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IIdentityGenerator<EmployeeId> _identityGenerator;
  private readonly ICompanyLicenseRetriever _companyLicenseRetriever;

  public EmployeeCreationCommandHandler(
    IIdentityGenerator<EmployeeId> identityGenerator,
    ICompanyLicenseRetriever companyLicenseRetriever,
    ICompanyQueryRepository companyQueryRepository,
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyLicenseRetriever = companyLicenseRetriever ?? throw new ArgumentNullException(nameof(companyLicenseRetriever));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _employeeQueryRepository =
      employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
  }

  public async Task<Unit> Handle(
    EmployeeCreationCommand request,
    CancellationToken cancellationToken)
  {
    var oldEntity = await _employeeQueryRepository.FindByUserIdAsync(request.UserID, cancellationToken)
      .ConfigureAwait(false);
    if (oldEntity.IsSome)
    {
      throw new EntityExistsException("An employee for this user has already been created.");
    }

    var companyEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    await companyEntity.Match(
        s => CreateEmployeeAsync(s, request, cancellationToken),
        () => throw new FailedDependencyException("A company for this tenant have not been created."))
      .ConfigureAwait(false);
    return Unit.Value;
  }

  private async Task CreateEmployeeAsync(
    CompanyEntity companyEntity,
    EmployeeCreationCommand request,
    CancellationToken cancellationToken)
  {
    await ValidateEntitlementsAsync(companyEntity.ID, cancellationToken).ConfigureAwait(false);
    var id = _identityGenerator.Generate();
    EmployeeEntity entity = new(
      id,
      request.TenantID,
      request.UserID,
      companyEntity.ID,
      None,
      None,
      request.FirstName,
      request.LastName,
      request.FullName);

    await _employeeRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
  }

  private static bool EnsureMaximumEmployeeCountWillNotExceed(
    int maximumEmployeeCount, int currentEmployeeCount)
  {
    return maximumEmployeeCount > currentEmployeeCount;
  }

  private async Task ValidateEntitlementsAsync(CompanyId companyId, CancellationToken cancellationToken)
  {
    var licenseValidation = await _companyLicenseRetriever.GetAsync(companyId, cancellationToken).ConfigureAwait(false);

    var currentEmployeeCount = await _employeeQueryRepository.CountAllAsync(companyId, cancellationToken).ConfigureAwait(false);

    _ = licenseValidation.Match(
        license =>
          Success<Error, License<CompanyEntitlements>>(license)
            .Validate(
              x => EnsureMaximumEmployeeCountWillNotExceed(x.Entitlements.MaximumEmployeeCount, currentEmployeeCount),
                43705651,
                "The current company license entitlements limit the number of employees that can be created, and this limit has been reached")
              .Map(_ => unit),
        _ =>
          EnsureMaximumEmployeeCountWillNotExceed(2, currentEmployeeCount)
            ? Success<Error, LanguageExt.Unit>(unit)
            : Fail<Error, LanguageExt.Unit>(Error.New(
              43722466,
              "The current company unlicensed and the maximum number of employees that can be created, and this limit has been reached. Please contact your system administrator to obtain a license for this company.")))
      .GetOrThrow();
  }
}
