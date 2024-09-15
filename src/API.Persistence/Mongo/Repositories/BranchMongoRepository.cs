using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class BranchMongoRepository
  : MongoRepository<BranchMongoEntity, long>, IBranchMongoRepository
{
  public BranchMongoRepository(
    IMongoClientSessionProvider mongoClientSessionProvider,
    IMongoDatabaseProvider mongoDatabaseProvider) : base(
    mongoClientSessionProvider,
    mongoDatabaseProvider,
    "Branches")
  {
  }

  protected override SortDefinition<BranchMongoEntity> PageSortDefinition
    => Builders<BranchMongoEntity>.Sort.Ascending(x => x.ID);

  public Task<PageResult<BranchMongoEntity>> PageAsync(
    TenantBranchPageQuery pageQuery,
    CancellationToken cancellationToken)
  {
    var filter =
      Builders<BranchMongoEntity>.Filter.Eq(item => item.TenantID, pageQuery.TenantId);

    return this.PageAsync(filter, pageQuery, cancellationToken);
  }
}
