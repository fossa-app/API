using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyCreationCommand(
  Guid TenantID,
  string Name)
  : EntityTenantCommand<CompanyEntity, long, Guid>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<long>();
}
