using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentModificationCommandHandler : IRequestHandler<DepartmentModificationCommand, Unit>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IPublisher _publisher;

  public DepartmentModificationCommandHandler(
      IDepartmentQueryRepository departmentQueryRepository,
      IDepartmentRepository departmentRepository,
      IPublisher publisher)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
      DepartmentModificationCommand request,
      CancellationToken cancellationToken)
  {
    var originalEntity = await _departmentQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    var entity = originalEntity with
    {
      Name = request.Name,
      ParentDepartmentId = request.ParentDepartmentId,
      ManagerId = request.ManagerId
    };
    await _departmentRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new DepartmentUpdatedEvent(
      entity.TenantID,
      entity.ID,
      entity.CompanyId,
      entity.Name,
      entity.ParentDepartmentId,
      entity.ManagerId);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);

    if (originalEntity.ParentDepartmentId != entity.ParentDepartmentId)
    {
      var hierarchyChangedEvent = new DepartmentHierarchyChangedEvent(
        entity.TenantID,
        entity.ID,
        entity.CompanyId,
        originalEntity.ParentDepartmentId,
        entity.ParentDepartmentId);

      await _publisher.Publish(hierarchyChangedEvent, cancellationToken).ConfigureAwait(false);
    }

    if (originalEntity.ManagerId != entity.ManagerId)
    {
      var managerChangedEvent = new DepartmentManagerChangedEvent(
        entity.TenantID,
        entity.ID,
        entity.CompanyId,
        originalEntity.ManagerId,
        entity.ManagerId);

      await _publisher.Publish(managerChangedEvent, cancellationToken).ConfigureAwait(false);
    }

    return Unit.Value;
  }
}
