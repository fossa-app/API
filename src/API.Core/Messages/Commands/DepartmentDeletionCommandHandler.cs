using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentDeletionCommandHandler : IRequestHandler<DepartmentDeletionCommand, Unit>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IRelationshipGraph _relationshipGraph;

  public DepartmentDeletionCommandHandler(
      IDepartmentQueryRepository departmentQueryRepository,
      IDepartmentRepository departmentRepository,
      IRelationshipGraph relationshipGraph)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
  }

  public async Task DeleteDepartmentAsync(
      DepartmentEntity entity,
      CancellationToken cancellationToken)
  {
    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _departmentRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
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
