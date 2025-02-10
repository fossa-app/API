using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeManagementCommand(
  Guid TenantID,
  Guid UserID,
  Option<BranchId> AssignedBranchId)
  : EntityTenantUserCommand<EmployeeEntity, EmployeeId, Guid, Guid>(TenantID, UserID)
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => [];
}
