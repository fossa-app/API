using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
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

    var options = new InstrumentationOptions { CaptureCommandText = true };
    mongoClientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(options));

    BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
  }
}
