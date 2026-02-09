using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanySettingsModificationCommand(
    Guid TenantID,
    Guid UserID,
    ColorSchemeId ColorSchemeId)
  : TenantUserEntityCommand<CompanySettingsEntity, CompanySettingsId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<CompanySettingsId> TenantEntityIdentities
    => [];
}
