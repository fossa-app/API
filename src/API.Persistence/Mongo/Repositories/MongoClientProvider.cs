using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class MongoClientProvider : MongoClientProviderBase
{
  public MongoClientProvider(IConfiguration configuration) : base(configuration, "MongoDB")
  {
  }
}
