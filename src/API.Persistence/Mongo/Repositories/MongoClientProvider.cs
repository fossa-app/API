using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class MongoClientProvider : MongoClientProviderBase
{
  public MongoClientProvider(IConfiguration configuration) : base(configuration, "MongoDB")
  {
  }

  protected override void ConfigureClientSettings(MongoClientSettings mongoClientSettings)
  {
    base.ConfigureClientSettings(mongoClientSettings);

#pragma warning disable CS0618 // Type or member is obsolete
    mongoClientSettings.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore CS0618 // Type or member is obsolete
  }
}
