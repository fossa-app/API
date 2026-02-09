using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeManagementCommand(
  EmployeeId ID,
  Guid TenantID,
  Guid UserID,
  Option<BranchId> AssignedBranchId,
  Option<DepartmentId> AssignedDepartmentId,
  Option<EmployeeId> ReportsToId,
  string JobTitle)
  : TenantUserEntityCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> TenantEntityIdentities
    => [ID];
}
