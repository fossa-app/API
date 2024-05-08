using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface ICompanyMongoRepository : IMongoRepository<CompanyMongoEntity, long>
{
  Task<Option<CompanyMongoEntity>> FindByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);

  Task<Option<CompanyMongoEntity>> FindByMonikerAsync(
      string moniker,
      CancellationToken cancellationToken);

  Task<CompanyMongoEntity> GetByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);

  Task<int> CountAllAsync(CancellationToken cancellationToken);
}
