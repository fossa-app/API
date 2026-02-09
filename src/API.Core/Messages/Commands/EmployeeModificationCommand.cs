using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeModificationCommand(
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : TenantUserEntityCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> TenantEntityIdentities
    => [];
}
