using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface IDepartmentQueryRepository
  : IQueryRepository<DepartmentEntity, DepartmentId>
  , IDependencyQueryRepository<CompanyId>
  , IDependencyQueryRepository<DepartmentId>
  , IDependencyQueryRepository<EmployeeId>
{
  Task<PageResult<DepartmentEntity>> PageAsync(
      TenantDepartmentPageQuery pageQuery,
      CancellationToken cancellationToken);
}
