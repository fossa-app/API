using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface ICompanySettingsQueryRepository
  : IQueryRepository<CompanySettingsEntity, CompanySettingsId>
  , IDependencyQueryRepository<CompanyId>
{
  Task<Option<CompanySettingsEntity>> FindByCompanyIdAsync(
      CompanyId companyId,
      CancellationToken cancellationToken);

  Task<CompanySettingsEntity> GetByCompanyIdAsync(
      CompanyId companyId,
      CancellationToken cancellationToken);
}
