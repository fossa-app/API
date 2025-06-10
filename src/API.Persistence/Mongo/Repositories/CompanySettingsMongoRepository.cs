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

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    var indexKeysDefinition =
      Builders<CompanySettingsMongoEntity>
      .IndexKeys
      .Ascending(x => x.CompanyId);
    var indexOptions = new CreateIndexOptions
    {
      Name = "company_id_index",
      Unique = true, // Each company can have only one set of settings
      Background = true
    };
    var indexModel = new CreateIndexModel<CompanySettingsMongoEntity>(indexKeysDefinition, indexOptions);
    return Collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
  }

  public async Task<bool> HasDependencyOnCompanyAsync(
    long companyId,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<CompanySettingsMongoEntity>.Filter.Eq(item => item.CompanyId, companyId);

    var count = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
    return count > 0;
  }
}
