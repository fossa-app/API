using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TIKSN.Data;

namespace Fossa.API.Persistence.Mongo.Entities;

public class EmployeeMongoEntity : IEntity<long>
{
  [BsonId]
  public long ID { get; set; }

  [BsonGuidRepresentation(GuidRepresentation.Standard)]
  public Guid TenantID { get; set; }

  [BsonGuidRepresentation(GuidRepresentation.Standard)]
  public Guid UserID { get; set; }

  public long CompanyId { get; set; }

  public long? AssignedBranchId { get; set; }

  public string? FirstName { get; set; }

  public string? LastName { get; set; }

  public string? FullName { get; set; }
}
