using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace Fossa.API.Persistence.Mongo.Entities;

public class SystemPropertiesMongoEntity : IEntity<long>
{
  [BsonId] public long ID { get; set; }

  public byte[]? SystemID { get; set; }
}
