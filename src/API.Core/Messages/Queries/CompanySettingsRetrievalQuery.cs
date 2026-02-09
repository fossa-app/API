using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanySettingsRetrievalQuery(
    Guid TenantID)
  : TenantEntityQuery<CompanySettingsEntity, CompanySettingsId, Guid, CompanySettingsEntity>(TenantID)
{
  public override IEnumerable<CompanySettingsId> TenantEntityIdentities
    => [];
}
