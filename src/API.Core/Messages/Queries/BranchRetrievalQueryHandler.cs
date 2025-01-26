using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class BranchRetrievalQueryHandler : IRequestHandler<BranchRetrievalQuery, BranchEntity>
{
  private readonly IBranchQueryRepository _branchQueryRepository;

  public BranchRetrievalQueryHandler(
    IBranchQueryRepository branchQueryRepository)
  {
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
  }

  public async Task<BranchEntity> Handle(
    BranchRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    return await _branchQueryRepository.GetAsync(
        request.ID, cancellationToken)
      .ConfigureAwait(false);
  }
}
