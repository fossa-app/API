using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Licensing;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentCreationCommandHandler : IRequestHandler<DepartmentCreationCommand, Unit>
{
  private readonly ICompanyLicenseRetriever _companyLicenseRetriever;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IDepartmentQueryRepository _departmentQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IIdentityGenerator<DepartmentId> _identityGenerator;

  public DepartmentCreationCommandHandler(
      IIdentityGenerator<DepartmentId> identityGenerator,
      IDepartmentRepository departmentRepository,
      ICompanyLicenseRetriever companyLicenseRetriever,
      IDepartmentQueryRepository departmentQueryRepository,
      ICompanyQueryRepository companyQueryRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    _companyLicenseRetriever = companyLicenseRetriever ?? throw new ArgumentNullException(nameof(companyLicenseRetriever));
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
  }

  public async Task<Unit> Handle(
    DepartmentCreationCommand request,
    CancellationToken cancellationToken)
  {
    var company = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    await ValidateEntitlementsAsync(company.ID, cancellationToken).ConfigureAwait(false);

    var id = _identityGenerator.Generate();
    DepartmentEntity entity = new(id, request.TenantID, company.ID, request.Name, request.ManagerId, request.ParentDepartmentId);

    await _departmentRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }

  private static bool EnsureMaximumDepartmentCountWillNotExceed(
      int maximumDepartmentCount, int currentDepartmentCount)
  {
    return maximumDepartmentCount > currentDepartmentCount;
  }

  private async Task ValidateEntitlementsAsync(CompanyId companyId, CancellationToken cancellationToken)
  {
    var licenseValidation = await _companyLicenseRetriever.GetAsync(companyId, cancellationToken).ConfigureAwait(false);

    var currentDepartmentCount = await _departmentQueryRepository.CountAllAsync(companyId, cancellationToken).ConfigureAwait(false);

    _ = licenseValidation.Match(
        license =>
            Success<Error, License<CompanyEntitlements>>(license)
                .Validate(
                    x => EnsureMaximumDepartmentCountWillNotExceed(x.Entitlements.MaximumDepartmentCount, currentDepartmentCount),
                    43705653,
                    "The current company license entitlements limit the number of departments that can be created, and this limit has been reached")
                .Map(_ => unit),
        _ =>
            EnsureMaximumDepartmentCountWillNotExceed(UnlicensedCompanyEntitlements.MaximumDepartmentCount, currentDepartmentCount)
                ? Success<Error, LanguageExt.Unit>(unit)
                : Fail<Error, LanguageExt.Unit>(Error.New(
                    43722468,
                    "The current company is unlicensed and the maximum number of departments that can be created has been reached. Please contact your system administrator to obtain a license for this company.")))
        .GetOrThrow();
  }
}
