using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeDeletionCommand(
    Guid TenantID,
    Guid UserID)
  : TenantUserEntityCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> TenantEntityIdentities
    => [];
}
