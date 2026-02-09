using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record BranchDeletionCommand(
  BranchId ID,
  Guid TenantID,
  Guid UserID)
: TenantEntityCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> TenantEntityIdentities
    => Prelude.Seq1(ID);
}
