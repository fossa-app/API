using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record BranchDeletionCommand(
  BranchId ID,
  Guid TenantID,
  Guid UserID)
: EntityTenantCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> TenantEntityReferencesIdentities
    => Prelude.Seq1(ID);
}
