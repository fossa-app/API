using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record BranchCreationCommand(
  Guid TenantID,
  Guid UserID,
  string Name)
: EntityTenantCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> AffectingTenantEntitiesIdentities
    => [];
}
