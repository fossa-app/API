using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeCreationCommand(
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : EntityTenantCommand<CompanyEntity, long, Guid>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<long>();
}
