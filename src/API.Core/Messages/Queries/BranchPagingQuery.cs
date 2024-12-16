using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public record BranchPagingQuery(
    Guid TenantID,
    Guid UserID,
    string Search,
    Page Page)
  : EntityTenantQuery<BranchEntity, BranchId, Guid, PageResult<BranchEntity>>(TenantID)
    , IPagingQuery<BranchEntity>
{
  public override IEnumerable<BranchId> AffectingTenantEntitiesIdentities
    => [];
}
