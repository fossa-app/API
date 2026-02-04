using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeDeletionCommandHandler : IRequestHandler<EmployeeDeletionCommand, Unit>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IRelationshipGraph _relationshipGraph;
  private readonly IPublisher _publisher;

  public EmployeeDeletionCommandHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository,
    IRelationshipGraph relationshipGraph,
    IPublisher publisher)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task DeleteEmployeeAsync(
    EmployeeEntity entity,
    CancellationToken cancellationToken)
  {
    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _employeeRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

    var deletedEvent = new EmployeeDeletedEvent(
      entity.TenantID,
      entity.UserID,
      entity.ID,
      entity.CompanyId);

    await _publisher.Publish(deletedEvent, cancellationToken).ConfigureAwait(false);
  }

  public async Task<Unit> Handle(
    EmployeeDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var entityMaybe = await _employeeQueryRepository.FindByUserIdAsync(request.UserID, cancellationToken).ConfigureAwait(false);
    await entityMaybe.IfSomeAsync(
      entity => DeleteEmployeeAsync(entity, cancellationToken)).ConfigureAwait(false);
    return Unit.Value;
  }
}
