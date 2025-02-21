using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record BranchListingQuery(
    Seq<BranchId> Ids,
    Guid TenantID,
    Guid UserID)
  : EntityTenantQuery<BranchEntity, BranchId, Guid, Seq<BranchEntity>>(TenantID)
{
  public override IEnumerable<BranchId> AffectingTenantEntitiesIdentities
    => Ids;
}
