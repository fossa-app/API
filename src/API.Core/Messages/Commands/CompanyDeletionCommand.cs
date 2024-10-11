using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyDeletionCommand(
    Guid TenantID)
  : EntityTenantCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> AffectingTenantEntitiesIdentities
    => [];
}
