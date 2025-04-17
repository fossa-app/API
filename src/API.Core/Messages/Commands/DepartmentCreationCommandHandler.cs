using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentCreationCommandHandler : IRequestHandler<DepartmentCreationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IIdentityGenerator<DepartmentId> _identityGenerator;

  public DepartmentCreationCommandHandler(
      IIdentityGenerator<DepartmentId> identityGenerator,
      ICompanyQueryRepository companyQueryRepository,
      IDepartmentRepository departmentRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
  }

  public async Task<Unit> Handle(
    DepartmentCreationCommand request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    await companyEntity.Match(
        s => CreateDepartmentAsync(s, request, cancellationToken),
        () => throw new FailedDependencyException("A company for this tenant have not been created."))
      .ConfigureAwait(false);
    return Unit.Value;
  }

  private async Task<Unit> CreateDepartmentAsync(
      CompanyEntity companyEntity,
      DepartmentCreationCommand request,
      CancellationToken cancellationToken)
  {
    var id = _identityGenerator.Generate();
    DepartmentEntity entity = new(
        id,
        request.TenantID,
        companyEntity.ID,
        request.Name,
        request.ManagerId,
        request.ParentDepartmentId);

    await _departmentRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
