using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace Fossa.API.Persistence.Mongo.Entities;

public class CompanySettingsMongoEntity : IEntity<long>
{
  [BsonId]
  public long ID { get; set; }

  public long CompanyId { get; set; }

  public string? ColorSchemeId { get; set; }
}
