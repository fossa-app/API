using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public class EmployeePagingQueryHandler : IRequestHandler<EmployeePagingQuery, PageResult<EmployeeEntity>>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;

  public EmployeePagingQueryHandler(IEmployeeQueryRepository employeeQueryRepository)
  {
    _employeeQueryRepository =
      employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
  }

  public Task<PageResult<EmployeeEntity>> Handle(
    EmployeePagingQuery request,
    CancellationToken cancellationToken)
  {
    return _employeeQueryRepository.PageAsync(
      new TenantEmployeePageQuery(request.TenantID, request.Search, request.Page),
      cancellationToken);
  }
}
