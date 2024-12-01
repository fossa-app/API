using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanyRetrievalQuery(
    Guid TenantID)
  : EntityTenantQuery<CompanyEntity, CompanyId, Guid, CompanyEntity>(TenantID)
{
  public override IEnumerable<CompanyId> AffectingTenantEntitiesIdentities
    => [];
}
