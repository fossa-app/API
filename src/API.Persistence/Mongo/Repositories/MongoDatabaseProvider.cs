using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class MongoDatabaseProvider : MongoDatabaseProviderBase
{
  public MongoDatabaseProvider(
      IMongoClientProvider mongoClientProvider,
      IConfiguration configuration) : base(
      mongoClientProvider,
      configuration,
      "MongoDB")
  {
  }
}
