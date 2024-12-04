using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace Fossa.API.Persistence.Mongo.Entities;

public class CompanyMongoEntity : IEntity<long>
{
  [BsonId]
  public long ID { get; set; }

  public string? Name { get; set; }

  [BsonGuidRepresentation(GuidRepresentation.Standard)]
  public Guid TenantID { get; set; }

  public string? CountryCode { get; set; }
}
