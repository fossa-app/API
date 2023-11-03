using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeCreationCommand(
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : EntityTenantCommand<EmployeeEntity, EmployeeId, Guid>(TenantID)
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<EmployeeId>();
}
