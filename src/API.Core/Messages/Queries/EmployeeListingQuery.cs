using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeeListingQuery(
    Seq<EmployeeId> Ids,
    Guid TenantID,
    Guid UserID)
  : EntityTenantQuery<EmployeeEntity, EmployeeId, Guid, Seq<EmployeeEntity>>(TenantID)
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => Ids;
}
