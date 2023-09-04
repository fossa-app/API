using Fossa.API.Persistence.Mongo.Entities;
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
}
