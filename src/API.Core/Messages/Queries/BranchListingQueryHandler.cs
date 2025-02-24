using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class BranchListingQueryHandler : IRequestHandler<BranchListingQuery, Seq<BranchEntity>>
{
  private readonly IBranchQueryRepository _branchQueryRepository;

  public BranchListingQueryHandler(IBranchQueryRepository branchQueryRepository)
  {
    _branchQueryRepository =
      branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
  }

  public async Task<Seq<BranchEntity>> Handle(
    BranchListingQuery request,
    CancellationToken cancellationToken)
  {
    var branches = await _branchQueryRepository.ListAsync(request.Ids, cancellationToken).ConfigureAwait(false);
    return branches.ToSeq();
  }
}
