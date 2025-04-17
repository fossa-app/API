using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class DepartmentListingQueryHandler : IRequestHandler<DepartmentListingQuery, Seq<DepartmentEntity>>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;

  public DepartmentListingQueryHandler(IDepartmentQueryRepository departmentQueryRepository)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
  }

  public async Task<Seq<DepartmentEntity>> Handle(
      DepartmentListingQuery request,
      CancellationToken cancellationToken)
  {
    var departments = await _departmentQueryRepository.ListAsync(request.Ids, cancellationToken).ConfigureAwait(false);
    return departments.ToSeq();
  }
}
