using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanySettingsDeletionCommand(
    Guid TenantID,
    Guid UserID)
  : TenantUserEntityCommand<CompanySettingsEntity, CompanySettingsId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<CompanySettingsId> TenantEntityIdentities
    => [];
}
