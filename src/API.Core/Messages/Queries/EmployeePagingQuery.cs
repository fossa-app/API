using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeePagingQuery(
    Guid TenantID,
    Guid UserID,
    Page Page)
  : EntityTenantQuery<EmployeeEntity, EmployeeId, Guid, PageResult<EmployeeEntity>>(TenantID)
    , IPagingQuery<EmployeeEntity>
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<EmployeeId>();
}
