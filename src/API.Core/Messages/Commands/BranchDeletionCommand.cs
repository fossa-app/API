using Fossa.API.Core.Entities;
using LanguageExt;

namespace Fossa.API.Core.Messages.Commands;

public record BranchDeletionCommand(
  BranchId ID,
  Guid TenantID,
  Guid UserID)
: EntityTenantCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> AffectingTenantEntitiesIdentities
    => Prelude.Seq1(ID);
}
