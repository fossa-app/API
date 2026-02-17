using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeModificationCommandHandler : IRequestHandler<EmployeeModificationCommand, Unit>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IPublisher _publisher;

  public EmployeeModificationCommandHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository,
    IPublisher publisher)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    EmployeeModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _employeeQueryRepository.GetByUserIdAsync(request.UserID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      FullName = request.FullName,
    };
    await _employeeRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new EmployeeUpdatedEvent(
      entity.TenantID,
      entity.UserID,
      entity.ID,
      entity.FirstName,
      entity.LastName,
      entity.FullName,
      entity.JobTitle,
      entity.AssignedBranchId,
      entity.AssignedDepartmentId,
      entity.ReportsToId,
      entity.CompanyId);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
