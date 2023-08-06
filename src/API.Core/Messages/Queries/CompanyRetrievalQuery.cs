using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record CompanyRetrievalQuery(
  Guid TenantID)
  : EntityTenantQuery<CompanyEntity, long, Guid, CompanyEntity>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<long>();
}
