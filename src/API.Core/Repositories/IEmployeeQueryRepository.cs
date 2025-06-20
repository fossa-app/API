﻿using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface IEmployeeQueryRepository
  : IQueryRepository<EmployeeEntity, EmployeeId>
  , IDependencyQueryRepository<CompanyId>
  , IDependencyQueryRepository<BranchId>
  , IDependencyQueryRepository<DepartmentId>
{
  Task<Option<EmployeeEntity>> FindByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);

  Task<EmployeeEntity> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);

  Task<PageResult<EmployeeEntity>> PageAsync(
    TenantEmployeePageQuery pageQuery,
    CancellationToken cancellationToken);

  Task<int> CountAllAsync(
    CompanyId companyId,
    CancellationToken cancellationToken);
}
