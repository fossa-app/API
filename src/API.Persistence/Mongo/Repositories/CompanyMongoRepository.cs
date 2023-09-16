using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using static LanguageExt.Prelude;

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

  public async Task<Option<CompanyMongoEntity>> FindByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanyMongoEntity>.Filter.Eq(item => item.TenantID, tenantId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return Optional(entity);
  }

  public async Task<CompanyMongoEntity> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanyMongoEntity>.Filter.Eq(item => item.TenantID, tenantId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return entity ?? throw new EntityNotFoundException();
  }
}
