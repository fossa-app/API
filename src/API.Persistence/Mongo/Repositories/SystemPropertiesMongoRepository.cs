using Fossa.API.Persistence.Mongo.Entities;
using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class SystemPropertiesMongoRepository
  : MongoRepository<SystemPropertiesMongoEntity, long>, ISystemPropertiesMongoRepository
{
  public SystemPropertiesMongoRepository(
    IMongoClientSessionProvider mongoClientSessionProvider,
    IMongoDatabaseProvider mongoDatabaseProvider) : base(
    mongoClientSessionProvider,
    mongoDatabaseProvider,
    "SystemProperties")
  {
  }

  protected override SortDefinition<SystemPropertiesMongoEntity> PageSortDefinition
    => Builders<SystemPropertiesMongoEntity>.Sort.Ascending(x => x.ID);
}
