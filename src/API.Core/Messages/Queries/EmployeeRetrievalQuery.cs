using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeeRetrievalQuery(
    Guid TenantID,
    Guid UserID)
  : EntityTenantQuery<EmployeeEntity, long, Guid, EmployeeEntity>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<long>();
}
