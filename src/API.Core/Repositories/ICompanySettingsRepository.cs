using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public interface ICompanySettingsRepository : IRepository<CompanySettingsEntity>
{
    Task<CompanySettingsEntity> GetByCompanyIdAsync(CompanyId companyId, CancellationToken cancellationToken);
    Task<Option<CompanySettingsEntity>> FindByCompanyIdAsync(CompanyId companyId, CancellationToken cancellationToken);
}
