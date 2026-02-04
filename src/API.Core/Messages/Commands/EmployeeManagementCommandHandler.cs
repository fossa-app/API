using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeManagementCommandHandler : IRequestHandler<EmployeeManagementCommand, Unit>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IPublisher _publisher;

  public EmployeeManagementCommandHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository,
    IPublisher publisher)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    EmployeeManagementCommand request,
    CancellationToken cancellationToken)
  {
    var originalEntity = await _employeeQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);

    var entity = originalEntity with
    {
      AssignedBranchId = request.AssignedBranchId,
      AssignedDepartmentId = request.AssignedDepartmentId,
      ReportsToId = request.ReportsToId,
      JobTitle = request.JobTitle
    };

    await _employeeRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new EmployeeUpdatedEvent(
      entity.TenantID,
      entity.UserID,
      entity.ID,
      entity.JobTitle,
      entity.AssignedBranchId,
      entity.AssignedDepartmentId,
      entity.ReportsToId,
      entity.CompanyId);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);

    if (originalEntity.AssignedBranchId != entity.AssignedBranchId)
    {
      var branchAssignmentEvent = new EmployeeAssignedToBranchEvent(
        entity.TenantID,
        entity.UserID,
        entity.ID,
        entity.CompanyId,
        originalEntity.AssignedBranchId,
        entity.AssignedBranchId);

      await _publisher.Publish(branchAssignmentEvent, cancellationToken).ConfigureAwait(false);
    }

    if (originalEntity.AssignedDepartmentId != entity.AssignedDepartmentId)
    {
      var departmentAssignmentEvent = new EmployeeAssignedToDepartmentEvent(
        entity.TenantID,
        entity.UserID,
        entity.ID,
        entity.CompanyId,
        originalEntity.AssignedDepartmentId,
        entity.AssignedDepartmentId);

      await _publisher.Publish(departmentAssignmentEvent, cancellationToken).ConfigureAwait(false);
    }

    if (originalEntity.ReportsToId != entity.ReportsToId)
    {
      var reportingStructureEvent = new EmployeeReportingStructureChangedEvent(
        entity.TenantID,
        entity.UserID,
        entity.ID,
        entity.CompanyId,
        originalEntity.ReportsToId,
        entity.ReportsToId);

      await _publisher.Publish(reportingStructureEvent, cancellationToken).ConfigureAwait(false);
    }

    return Unit.Value;
  }
}
