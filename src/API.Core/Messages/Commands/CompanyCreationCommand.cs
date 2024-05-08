using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyCreationCommand(
  Guid TenantID,
  string Name,
  string Moniker)
  : EntityTenantCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<CompanyId>();
}
