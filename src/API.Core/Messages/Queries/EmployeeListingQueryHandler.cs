using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class EmployeeListingQueryHandler : IRequestHandler<EmployeeListingQuery, Seq<EmployeeEntity>>
{
  private readonly IEmployeeQueryRepository _branchQueryRepository;

  public EmployeeListingQueryHandler(IEmployeeQueryRepository branchQueryRepository)
  {
    _branchQueryRepository =
      branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
  }

  public async Task<Seq<EmployeeEntity>> Handle(
    EmployeeListingQuery request,
    CancellationToken cancellationToken)
  {
    var branches = await _branchQueryRepository.ListAsync(request.Ids, cancellationToken).ConfigureAwait(false);
    return branches.ToSeq();
  }
}
