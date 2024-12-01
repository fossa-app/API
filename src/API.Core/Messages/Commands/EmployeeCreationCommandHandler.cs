using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Data;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeCreationCommandHandler : IRequestHandler<EmployeeCreationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IIdentityGenerator<EmployeeId> _identityGenerator;

  public EmployeeCreationCommandHandler(
    IIdentityGenerator<EmployeeId> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
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
    var id = _identityGenerator.Generate();
    EmployeeEntity entity = new(
      id,
      request.TenantID,
      request.UserID,
      companyEntity.ID,
      request.FirstName,
      request.LastName,
      request.FullName);

    await _employeeRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
  }
}
