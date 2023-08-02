using Fossa.API.Persistence.Mongo.Entities;
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
}
