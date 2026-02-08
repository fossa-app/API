using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record DepartmentRetrievalQuery(
    DepartmentId ID,
    Guid TenantID,
    Guid UserID)
    : EntityTenantQuery<DepartmentEntity, DepartmentId, Guid, DepartmentEntity>(TenantID)
{
  public override IEnumerable<DepartmentId> TenantEntityReferencesIdentities
      => [ID];
}
