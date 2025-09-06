using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Core.Messages.Queries;

public class EmployeePagingQueryHandler : IRequestHandler<EmployeePagingQuery, PageResult<EmployeeEntity>>
{
  private readonly IMapper<EmployeeId, long> _employeeModelToDataIdentityMapper;
  private readonly IEmployeeQueryRepository _employeeQueryRepository;

  public EmployeePagingQueryHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IMapper<EmployeeId, long> employeeModelToDataIdentityMapper)
  {
    _employeeQueryRepository =
      employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeModelToDataIdentityMapper = employeeModelToDataIdentityMapper ?? throw new ArgumentNullException(nameof(employeeModelToDataIdentityMapper));
  }

  public Task<PageResult<EmployeeEntity>> Handle(
    EmployeePagingQuery request,
    CancellationToken cancellationToken)
  {
    return _employeeQueryRepository.PageAsync(
      new TenantEmployeePageQuery(
        request.TenantID,
        request.Search,
        request.Page,
        request.ReportsToId.Map(_employeeModelToDataIdentityMapper.Map),
        request.TopLevelOnly),
      cancellationToken);
  }
}
