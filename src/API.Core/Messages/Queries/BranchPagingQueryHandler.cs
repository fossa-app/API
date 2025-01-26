using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public class BranchPagingQueryHandler : IRequestHandler<BranchPagingQuery, PageResult<BranchEntity>>
{
  private readonly IBranchQueryRepository _branchQueryRepository;

  public BranchPagingQueryHandler(IBranchQueryRepository branchQueryRepository)
  {
    _branchQueryRepository =
      branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
  }

  public Task<PageResult<BranchEntity>> Handle(
    BranchPagingQuery request,
    CancellationToken cancellationToken)
  {
    return _branchQueryRepository.PageAsync(
      new TenantBranchPageQuery(request.TenantID, request.Search, request.Page),
      cancellationToken);
  }
}
