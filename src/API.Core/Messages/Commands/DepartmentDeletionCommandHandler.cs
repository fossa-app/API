using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentDeletionCommandHandler : IRequestHandler<DepartmentDeletionCommand, Unit>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IRelationshipGraph _relationshipGraph;
  private readonly IPublisher _publisher;

  public DepartmentDeletionCommandHandler(
      IDepartmentQueryRepository departmentQueryRepository,
      IDepartmentRepository departmentRepository,
      IRelationshipGraph relationshipGraph,
      IPublisher publisher)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task DeleteDepartmentAsync(
      DepartmentEntity entity,
      CancellationToken cancellationToken)
  {
    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _departmentRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

    var deletedEvent = new DepartmentDeletedEvent(
      entity.TenantID,
      entity.ID,
      entity.CompanyId);

    await _publisher.Publish(deletedEvent, cancellationToken).ConfigureAwait(false);
  }

  public async Task<Unit> Handle(
      DepartmentDeletionCommand request,
      CancellationToken cancellationToken)
  {
    var entityMaybe = await _departmentQueryRepository.GetOrNoneAsync(request.ID, cancellationToken).ConfigureAwait(false);
    await entityMaybe.IfSomeAsync(
        entity => DeleteDepartmentAsync(entity, cancellationToken)).ConfigureAwait(false);
    return Unit.Value;
  }
}
