using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class CompanySettingsMongoRepository
  : MongoRepository<CompanySettingsMongoEntity, long>, ICompanySettingsMongoRepository
{
  public CompanySettingsMongoRepository(
    IMongoClientSessionProvider mongoClientSessionProvider,
    IMongoDatabaseProvider mongoDatabaseProvider) : base(
    mongoClientSessionProvider,
    mongoDatabaseProvider,
    "CompanySettings")
  {
  }

  protected override SortDefinition<CompanySettingsMongoEntity> PageSortDefinition
    => Builders<CompanySettingsMongoEntity>.Sort.Ascending(x => x.ID);

  public async Task<Option<CompanySettingsMongoEntity>> FindByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanySettingsMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return Optional(entity);
  }

  public async Task<CompanySettingsMongoEntity> GetByCompanyIdAsync(
    long companyId,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanySettingsMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var entity = await SingleOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);

    return entity ?? throw new EntityNotFoundException();
  }
}
