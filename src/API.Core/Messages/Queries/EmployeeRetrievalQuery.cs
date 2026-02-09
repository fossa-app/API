using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeeRetrievalQuery(
    EmployeeId ID,
    Guid TenantID,
    Guid UserID)
  : TenantEntityQuery<EmployeeEntity, EmployeeId, Guid, EmployeeEntity>(TenantID)
{
  public override IEnumerable<EmployeeId> TenantEntityIdentities
    => [ID];
}
