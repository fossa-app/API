using Fossa.API.Core.Entities;
using NodaTime;

namespace Fossa.API.Core.Messages.Commands;

public record BranchModificationCommand(
  BranchId ID,
  Guid TenantID,
  Guid UserID,
  string Name,
  DateTimeZone TimeZone,
  Option<Address> Address)
: EntityTenantCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> TenantEntityReferencesIdentities
    => Prelude.Seq1(ID);
}
