using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeCreationCommand(
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : EntityTenantUserCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> TenantEntityReferencesIdentities
    => [];
}
