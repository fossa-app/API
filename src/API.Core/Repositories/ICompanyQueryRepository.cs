using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface ICompanyQueryRepository : IQueryRepository<CompanyEntity, long>
{
  Task<CompanyEntity> GetByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);
}
