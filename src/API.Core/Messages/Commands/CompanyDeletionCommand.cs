using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyDeletionCommand(
    Guid TenantID)
  : TenantEntityCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> TenantEntityIdentities
    => [];
}
