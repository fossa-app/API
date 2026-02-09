using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanyRetrievalQuery(
    Guid TenantID)
  : TenantEntityQuery<CompanyEntity, CompanyId, Guid, CompanyEntity>(TenantID)
{
  public override IEnumerable<CompanyId> TenantEntityIdentities
    => [];
}
