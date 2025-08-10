using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeManagementCommandHandler : IRequestHandler<EmployeeManagementCommand, Unit>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public EmployeeManagementCommandHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
  }

  public async Task<Unit> Handle(
    EmployeeManagementCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _employeeQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      AssignedBranchId = request.AssignedBranchId,
      AssignedDepartmentId = request.AssignedDepartmentId
    };
    await _employeeRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
