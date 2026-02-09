using Fossa.API.Core.Entities;
using TIKSN.Data;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Core.Messages.Queries;

public record EmployeePagingQuery(
    Guid TenantID,
    Guid UserID,
    string Search,
    Option<EmployeeId> ReportsToId,
    bool TopLevelOnly,
    Page Page)
  : TenantEntityQuery<EmployeeEntity, EmployeeId, Guid, PageResult<EmployeeEntity>>(TenantID)
    , IPagingQuery<EmployeeEntity>
{
  public override IEnumerable<EmployeeId> TenantEntityIdentities
    => [];
}
