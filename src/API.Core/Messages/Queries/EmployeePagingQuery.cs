using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeePagingQuery(
    Guid TenantID,
    Guid UserID,
    Page Page)
  : EntityTenantQuery<EmployeeEntity, long, Guid, PageResult<EmployeeEntity>>(TenantID)
  , IPagingQuery<EmployeeEntity>
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Enumerable.Empty<long>();
}
