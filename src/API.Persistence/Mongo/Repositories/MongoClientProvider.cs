using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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

    BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
  }
}
