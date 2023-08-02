using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface ICompanyMongoRepository : IMongoRepository<CompanyMongoEntity, long>
{
  Task<CompanyMongoEntity> GetByTenantIdAsync(
    Guid tenantId,
    CancellationToken cancellationToken);
}
