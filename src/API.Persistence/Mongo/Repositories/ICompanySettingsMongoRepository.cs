using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface ICompanySettingsMongoRepository : IMongoRepository<CompanySettingsMongoEntity, long>
{
  Task<Option<CompanySettingsMongoEntity>> FindByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken);

  Task<CompanySettingsMongoEntity> GetByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken);

  Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken);

  Task<bool> HasDependencyOnCompanyAsync(
    long companyId,
    CancellationToken cancellationToken);
}
