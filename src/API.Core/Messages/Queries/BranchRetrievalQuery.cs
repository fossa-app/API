using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record BranchRetrievalQuery(
    BranchId ID,
    Guid TenantID,
    Guid UserID)
  : TenantEntityQuery<BranchEntity, BranchId, Guid, BranchEntity>(TenantID)
{
  public override IEnumerable<BranchId> TenantEntityIdentities
    => [ID];
}
