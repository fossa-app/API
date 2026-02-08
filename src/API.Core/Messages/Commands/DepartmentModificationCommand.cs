using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record DepartmentModificationCommand(
    DepartmentId ID,
    Guid TenantID,
    Guid UserID,
    string Name,
    Option<DepartmentId> ParentDepartmentId,
    EmployeeId ManagerId)
    : EntityTenantCommand<DepartmentEntity, DepartmentId, Guid>(TenantID)
{
  public override IEnumerable<DepartmentId> TenantEntityReferencesIdentities
      => Seq1(ID);
}
