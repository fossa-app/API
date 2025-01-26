using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface ICompanyQueryRepository : IQueryRepository<CompanyEntity, CompanyId>
{
  Task<Option<CompanyEntity>> FindByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);

  Task<CompanyEntity> GetByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);

  Task<int> CountAllAsync(CancellationToken cancellationToken);
}
