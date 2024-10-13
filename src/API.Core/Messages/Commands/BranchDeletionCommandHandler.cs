using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public class BranchDeletionCommandHandler : IRequestHandler<BranchDeletionCommand, Unit>
{
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly IBranchRepository _branchRepository;
  private readonly IRelationshipGraph _relationshipGraph;

  public BranchDeletionCommandHandler(
    IBranchQueryRepository branchQueryRepository,
    IBranchRepository branchRepository,
    IRelationshipGraph relationshipGraph)
  {
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
  }

  public async Task DeleteBranchAsync(
    BranchEntity entity,
    CancellationToken cancellationToken)
  {
    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _branchRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
  }

  public async Task<Unit> Handle(
    BranchDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var entityMaybe = await _branchQueryRepository.GetOrNoneAsync(request.ID, cancellationToken).ConfigureAwait(false);
    await entityMaybe.IfSomeAsync(
      entity => DeleteBranchAsync(entity, cancellationToken)).ConfigureAwait(false);
    return Unit.Value;
  }
}
