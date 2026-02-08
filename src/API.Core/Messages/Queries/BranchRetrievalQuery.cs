using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record BranchRetrievalQuery(
    BranchId ID,
    Guid TenantID,
    Guid UserID)
  : EntityTenantQuery<BranchEntity, BranchId, Guid, BranchEntity>(TenantID)
{
  public override IEnumerable<BranchId> TenantEntityReferencesIdentities
    => [ID];
}
