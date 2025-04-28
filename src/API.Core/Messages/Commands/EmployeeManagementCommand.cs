using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeManagementCommand(
  EmployeeId ID,
  Guid TenantID,
  Guid UserID,
  Option<BranchId> AssignedBranchId,
  Option<DepartmentId> AssignedDepartmentId)
  : EntityTenantUserCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => [ID];
}
