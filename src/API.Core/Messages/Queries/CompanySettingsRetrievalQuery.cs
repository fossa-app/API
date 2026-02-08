using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanySettingsRetrievalQuery(
    Guid TenantID)
  : EntityTenantQuery<CompanySettingsEntity, CompanySettingsId, Guid, CompanySettingsEntity>(TenantID)
{
  public override IEnumerable<CompanySettingsId> TenantEntityReferencesIdentities
    => [];
}
