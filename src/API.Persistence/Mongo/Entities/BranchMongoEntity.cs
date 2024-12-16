using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace Fossa.API.Persistence.Mongo.Entities;

public class BranchMongoEntity : IEntity<long>
{
  [BsonId]
  public long ID { get; set; }

  [BsonGuidRepresentation(GuidRepresentation.Standard)]
  public Guid TenantID { get; set; }

  public long CompanyId { get; set; }

  public string? Name { get; set; }

  public string? TimeZoneId { get; set; }
}
