using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public class DepartmentPagingQueryHandler : IRequestHandler<DepartmentPagingQuery, PageResult<DepartmentEntity>>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;

  public DepartmentPagingQueryHandler(IDepartmentQueryRepository departmentQueryRepository)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
  }

  public Task<PageResult<DepartmentEntity>> Handle(
      DepartmentPagingQuery request,
      CancellationToken cancellationToken)
  {
    return _departmentQueryRepository.PageAsync(
        new TenantDepartmentPageQuery(request.TenantID, request.Search, request.Page),
        cancellationToken);
  }
}
