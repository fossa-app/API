using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class BranchModificationCommandHandler : IRequestHandler<BranchModificationCommand, Unit>
{
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly IBranchRepository _branchRepository;

  public BranchModificationCommandHandler(
    IBranchQueryRepository branchQueryRepository,
    IBranchRepository branchRepository)
  {
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
  }

  public async Task<Unit> Handle(
    BranchModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _branchQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      Name = request.Name,
      TimeZone = request.TimeZone,
      Address = request.Address,
    };
    await _branchRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
