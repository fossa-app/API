using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanySettingsCreationCommand(
    Guid TenantID,
    Guid UserID,
    ColorSchemeId ColorSchemeId)
  : EntityTenantUserCommand<CompanySettingsEntity, CompanySettingsId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<CompanySettingsId> AffectingTenantEntitiesIdentities
    => [];
}
