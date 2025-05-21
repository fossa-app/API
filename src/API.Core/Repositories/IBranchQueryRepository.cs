using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface IBranchQueryRepository : IQueryRepository<BranchEntity, BranchId>, IDependencyQueryRepository<CompanyId>
{
  Task<PageResult<BranchEntity>> PageAsync(
    TenantBranchPageQuery pageQuery,
    CancellationToken cancellationToken);

  Task<int> CountAllAsync(
    CompanyId companyId,
    CancellationToken cancellationToken);
}
