using Fossa.API.Core;
using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class CompanyMongoRepository
  : MongoRepository<CompanyMongoEntity, long>, ICompanyMongoRepository
{
  public CompanyMongoRepository(
      IMongoClientSessionProvider mongoClientSessionProvider,
      IMongoDatabaseProvider mongoDatabaseProvider) : base(
          mongoClientSessionProvider,
          mongoDatabaseProvider,
          "Companies")
  {
  }

  public async Task<CompanyMongoEntity> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanyMongoEntity>.Filter.Eq(item => item.TenantID, tenantId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return entity ?? throw new EntityNotFoundException();
  }
}
