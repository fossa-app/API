using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanySettingsDeletionCommand(
    CompanySettingsId ID,
    Guid TenantID,
    Guid UserID)
  : EntityTenantUserCommand<CompanySettingsEntity, CompanySettingsId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<CompanySettingsId> AffectingTenantEntitiesIdentities
    => Seq1(ID);
}
